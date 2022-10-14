using ConceptDbLib.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConceptDbLib.Services
{
    public class BuilderService
    {
        private readonly ConceptContext _db;
        private readonly Dictionary<string, CptObjLibrary> _libs;
        private readonly Dictionary<string, CptNetwork> _networks;
        private readonly Dictionary<string, CptObject> _objects;
        private readonly Dictionary<string, bool> _objectSaved;

        private static ConceptDbResponse SomeObjectsNotInNetwork => new(ConceptDbResponseId.Error, "Some Objects Not In Network", new() { });
        private static ConceptDbResponse LibCreated(string name) => new(ConceptDbResponseId.Success, "Library created", new() { name });
        private static ConceptDbResponse LibNameNotAvailable(string name) => new(ConceptDbResponseId.Error, "Library Not Created. Another library already exists with the same name {0}.", new() { name });
        private static ConceptDbResponse LibNotFound(string name) => new(ConceptDbResponseId.Error, "Library Not Found", new() { name });
        private static ConceptDbResponse ObjCreated(string name) => new(ConceptDbResponseId.Success, "Object Created. Object will only be saved once added to an object library. Use the 'AddObjectToLibrary' or 'AddObjectToLibraryAsync' methods to add this object to an existing object library. To create a new object library, use the 'CreateObjectLibrary' or 'CreateObjectLibraryAsync' methods.", new() { name });
        private static ConceptDbResponse ObjNameUnavailable(string name) => new(ConceptDbResponseId.Error, "Object Not Created. Another object with the same name already exists on this network.", new() { name });
        private static ConceptDbResponse ObjNotFound(string name) => new(ConceptDbResponseId.Error, "Object Not Found", new() { name });
        private static ConceptDbResponse NetworkCreated(string name) => new(ConceptDbResponseId.Success, "Network Created Successfully", new() { name });
        private static ConceptDbResponse NetworkAlreadyExists(string name) => new(ConceptDbResponseId.Error, "Network Not Created. Already Exists.", new() { name });
        private static ConceptDbResponse ObjectAddedToLibrary(string objName, string libName) => new(ConceptDbResponseId.Success, "Object Added to Object Library. Value0 = Object, Value1 = Library", new() { objName, libName });
        private static ConceptDbResponse ObjectAddedToNetwork(string objName, string netName) => new(ConceptDbResponseId.Success, "Object Added to Network. Value0 = Object, Value1 = Network", new() { objName, netName });
        private static ConceptDbResponse ObjectNotAddedToNetworkNotSaved(string objName) => new(ConceptDbResponseId.Error, "Object not added to network. Object must first be saved to an existing library. Use the 'AddObjectToLibrary' or 'AddObjectToLibraryAsync' methods to add this object to an existing object library. To create a new object library, use the 'CreateObjectLibrary' or 'CreateObjectLibraryAsync' methods.", new() { objName });
        private static ConceptDbResponse ObjectAlreadyInLibrary(string objName, string libName) => new(ConceptDbResponseId.Error, "Object Already in Library. Value0 = Object, Value1 = Library", new() { objName, libName });
        private static ConceptDbResponse ObjectAlreadyInNetwork(string objName, string networkName) => new(ConceptDbResponseId.Error, "Object Already in Network. Value0 = Object, Value1 = Network", new() { objName, networkName });
        private static ConceptDbResponse NetworkNotFound(string networkName) => new(ConceptDbResponseId.Error, "Network Not Found", new() { networkName });
        private static ConceptDbResponse NetworkMembershipCreated(string networkName, string parentObj, string childObj) => new(ConceptDbResponseId.Success, "Network Membership Created 1: Network, 2: ParentObj, 3: ChildObj", new() { networkName, parentObj, childObj });
        public BuilderService(ConceptContext db)
        {
            _db = db;
            _libs = new();
            _objects = new();
            _objectSaved = new();
            _networks = new();
            int lCount = db.ConceptObjectLibs.Count();
            List<CptObjLibrary> objLibs = db.ConceptObjectLibs.ToList();
            for(int i = 0; i < lCount; i++)
            {
                CptObjLibrary objLib = objLibs[i];
                int oCount = objLib.Objects.Count;
                for (int j = 0; j < oCount; j++)
                {
                    CptObject obj = objLib.Objects[j];
                    _objects[obj.Name] = obj;
                    _objectSaved[obj.Name] = true;
                }
            }
            foreach (CptNetwork n in db.ConceptNetworks)
            {
                _networks[n.Name] = n;
            }
        }

        public ConceptDbResponse CreateNetwork(string name)
        {
            bool exists = _networks.ContainsKey(name);
            if (!exists)
            {
                CptNetwork network = new(name);
                _db.ConceptNetworks.Add(network);
                _networks[name] = network;
                _db.SaveChanges();
                return NetworkCreated(name);
            }
            else
            {
                return NetworkAlreadyExists(name);
            }
        }

        public ConceptDbResponse CreateObjectLibrary(string name)
        {
            bool exists = _libs.ContainsKey(name);
            if (!exists)
            {
                CptObjLibrary newLib = new(name);
                _libs[name] = newLib;
                _db.ConceptObjectLibs.Add(newLib);
                _db.SaveChanges();
                return LibCreated(name);
            }
            else
            {
                return LibNameNotAvailable(name);
            }
        }

        public ConceptDbResponse CreateObject(string name)
        {
            bool exists = _objects.ContainsKey(name);
            if (!exists)
            {
                CptObject newObj = new(name);
                _objects[name] = newObj;
                return ObjCreated(name);
            }
            else
            {
                return ObjNameUnavailable(name);
            }
        }

        public ConceptDbResponse AddObjectToLibrary(string objName, string libName)
        {
            bool objectExists = _objects.ContainsKey(objName);
            bool libraryExists = _libs.ContainsKey(libName);

            if (objectExists)
            {
                if (libraryExists)
                {
                    CptObject obj = _objects[objName];
                    CptObjLibrary objLib = _libs[libName];
                    bool already = objLib.Objects.Contains(obj);
                    if (!already)
                    {
                        objLib.Objects.Add(obj);
                        _objectSaved[objName] = true;
                        _db.SaveChanges();
                        return ObjectAddedToLibrary(objName, libName);
                    }
                    else
                    {
                        return ObjectAlreadyInLibrary(objName, libName);
                    }
                }
                else
                {
                    return LibNotFound(libName);
                }
            }
            else
            {
                return ObjNotFound(objName);
            }
        }

        

        public ConceptDbResponse AddObjectToNetwork(string objName, string networkName)
        {
            bool networkExists = _networks.ContainsKey(networkName);
            if (networkExists)
            {
                CptNetwork network = _networks[networkName];
                bool objExists = _objects.ContainsKey(objName);
                if (objExists)
                {
                    CptObject obj = _objects[objName];
                    bool already = network.Objects.Any(o => o.Name == objName);
                    if (!already)
                    {
                        network.Objects.Add(obj);
                        CptNetworkNode membership = new();
                        membership.Name = "NetworkMembership";
                        membership.CptObject = obj;
                        network.Nodes.Add(membership);
                        _db.SaveChanges();
                        return ObjectAddedToNetwork(objName, networkName);
                    }
                    else
                    {
                        return ObjectAlreadyInNetwork(objName, networkName);
                    }
                }
                else
                {
                    return ObjNotFound(objName);
                }
            }
            else
            {
                return NetworkNotFound(networkName);
            }
        }

        private static bool NetworkContainsObject(CptNetwork network, string objName) => network.Objects.Any(o => o.Name == objName);
        internal ConceptDbResponse CreateNetworkMembership(string networkName, string parentObj, string childObj)
        {
            bool networkExists = _networks.ContainsKey(networkName);
            if (networkExists)
            {
                CptNetwork network = _networks[networkName];
                bool parentObjExists = NetworkContainsObject(network, parentObj);
                if (parentObjExists)
                {
                    CptObject parentObject = _objects[parentObj];
                    CptNetworkNode parentMembership = parentObject.Nodes.Where(n => n.Name == "NetworkMembership").FirstOrDefault() ?? throw new InvalidOperationException("Network Membership Not Found");                    
                    bool childObjExists = NetworkContainsObject(network, childObj);
                    if (childObjExists)
                    {
                        CptObject childObject = _objects[childObj];
                        CptNetworkNode childMembership = childObject.Nodes.Where(n => n.Name == "NetworkMembership").FirstOrDefault() ?? throw new InvalidOperationException("Network Membership Not Found");
                        parentMembership.ChildNodes.Add(childMembership);
                        _db.SaveChanges();
                        return NetworkMembershipCreated(networkName, parentObj, childObj);
                    }
                    else
                    {
                        return ObjNotFound(childObj);
                    }
                }
                else
                {
                    return ObjNotFound(parentObj);
                }
            }
            else
            {
                return NetworkNotFound(networkName);
            }
        }
    }
}
