﻿using CptClientShared;
using CptClientShared.Entities;
using CptClientShared.Scopes;

namespace ConceptDbLib.Services
{
    public class BuilderService
    {
        private readonly CptDbProvider _dbProvider;
        private ConceptContext _db => _dbProvider.Context;       
        public BuilderService(CptDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        //OBJECT CRUD
        internal ConceptDbResponse RemoveObjectFromLibrary(string libName, string objName)
        {
            DbSearchResult libSearch = SearchLibrary(libName);
            DbSearchResult objSearch = SearchObject(libName, objName);
            if (objSearch.Found)
            {
                CptObject obj = objSearch.Objects[0];
                CptLibrary lib = libSearch.Libraries[0];
                lib.Objects.Remove(obj);
                _db.SaveChanges();
                return StaticMessages.ObjectRemovedFromLibrary(libName, objName);
            }
            else
            {
                return objSearch.ResultId switch
                {
                    ResultId.Unspecified => throw new InvalidOperationException("Result not populated."),
                    ResultId.Success => throw new InvalidOperationException("Unexpected operation."),
                    ResultId.LibNotFound => StaticMessages.LibraryNotFound(libName),
                    ResultId.ObjNotInLib => StaticMessages.ObjectNotFound(libName, objName),
                    _ => throw new InvalidOperationException("Invalid Value in Switch Case."),
                };
            }
        }
        internal ConceptDbResponse SeverParentChildObjectRelationship(string libName, string childName)
        {
            DbSearchResult libSearch = SearchLibrary(libName);
            if (libSearch.Found)
            {
                DbSearchResult childSearch = SearchObject(libName, childName);
                if (childSearch.Found)
                {
                    CptObject child = childSearch.Objects[0];
                    CptObject? parent = childSearch.Objects[0].Parent;
                    if(parent != null)
                    {
                        string pName = parent.Name;
                        child.Parent = null;
                        _db.SaveChanges();
                        return StaticMessages.ParentChildObjectRelationshipSevered(libName, pName, childName);

                    }
                    else
                    {
                        return StaticMessages.ObjectHasNoParent(libName, childName);
                    }
                }
                else
                {
                    return StaticMessages.ObjectNotFound(libName, childName);
                }
            }
            else
            {
                return StaticMessages.LibraryNotFound(libName);
            }
        }
        internal ConceptDbResponse NewParentChildObjRelationship(string libName, string parentName, string childName)
        {
            DbSearchResult libSearch = SearchLibrary(libName);
            if (libSearch.Found)
            {
                DbSearchResult parentObjSearch = SearchObject(libName, parentName);
                DbSearchResult childObjSearch = SearchObject(libName, childName);
                if (parentObjSearch.Found)
                {
                    if (childObjSearch.Found)
                    {
                        CptObject parent = parentObjSearch.Objects[0];
                        CptObject child = childObjSearch.Objects[0];
                        child.Parent = parent;
                        _db.SaveChanges();
                        return StaticMessages.ParentChildObjRelationshipAdded(libName, parentName, childName);
                    }
                    else
                    {
                        return StaticMessages.ObjectNotFound(libName, childName);
                    }
                }
                else
                {
                    return StaticMessages.ObjectNotFound(libName, parentName);
                }
            }
            else
            {
                return StaticMessages.LibraryNotFound(libName);
            }
        }
        internal ConceptDbResponse RenameObject(string libName, string oldName, string newName)
        {
            DbSearchResult libSearch = SearchLibrary(libName);
            if (libSearch.Found)
            {
                DbSearchResult objSearch = SearchObject(libName, oldName);
                if (objSearch.Found)
                {
                    CptObject obj = objSearch.Objects[0];
                    obj.Name = newName;
                    _db.SaveChanges();
                    return StaticMessages.ObjectRenamed(libName, oldName, newName);
                }
                else
                {
                    return StaticMessages.ObjectNotFound(libName, oldName);
                }
            }
            else
            {
                return StaticMessages.LibraryNotFound(libName);
            }

        }
        internal ConceptDbResponse NewObject(string libName, string objName, string parentObjName, string objTypes)
        {
            List<string> objTypeNames = objTypes == string.Empty ? new() { "root" } : objTypes.Split("_").ToList();
            DbSearchResult objSearch = SearchObject(libName,objName);
            if (objSearch.Found)
            {
                return StaticMessages.ObjectNameUnavailable(objName);
            }
            else
            {
                DbSearchResult libSearch = SearchLibrary(libName);
                if (libSearch.Found)
                {
                    CptObject newObj = new()
                    {
                        Name = objName
                    };
                    libSearch.Libraries[0].Objects.Add(newObj);
                    if(parentObjName != string.Empty)
                    {
                        DbSearchResult parentObjSearch = SearchObject(libName, parentObjName);
                        if (parentObjSearch.Found)
                        {
                            newObj.Parent = parentObjSearch.Objects[0];
                        }
                        else
                        {
                            return StaticMessages.ObjectNotFound(libName, parentObjName);
                        }
                    }
                    foreach(string objTypeName in objTypeNames)
                    {
                        DbSearchResult objTypeSearch = SearchObjType(libSearch.Libraries[0], objTypeName);
                        if (objTypeSearch.Found)
                        {
                            newObj.ObjectTypes.Add(objTypeSearch.ObjectTypes[0]);
                        }
                        else
                        {
                            return StaticMessages.ObjectTypeNotFound(libName, objTypeName);
                        }
                    }
                    _db.SaveChanges();
                    return StaticMessages.ObjectCreated(newObj.Id, newObj.Name);
                }
                else
                {
                    return StaticMessages.LibraryNotFound(libName);
                }
            }
        }

        private static CptObject? RetrieveObjectFromLibrary(CptLibrary library, string objName)
            => library.Objects.Where(o => o.Name == objName).FirstOrDefault();
        private DbSearchResult SearchObject(string libName, string objName)
        {
            DbSearchResult libSearch = SearchLibrary(libName);
            if (libSearch.Found)
            {
                CptObject? result = RetrieveObjectFromLibrary(libSearch.Libraries[0], objName);
                if(result != null)
                {
                    return new(true, ResultId.Success, new(), new() { result });
                }
                else
                {
                    return DbSearchResult.ObjNotInLib;
                }
            }
            else
            {
                return DbSearchResult.LibNotFound;
            }
        }

        //LIBRARY CRUD
        internal ConceptDbResponse RenameObjectType(string libName, string oldName, string newName)
        {
            DbSearchResult libSearch = SearchLibrary(libName);
            if (libSearch.Found)
            {
                CptLibrary lib = libSearch.Libraries[0];
                DbSearchResult existingType = SearchObjType(lib, newName);
                if (!existingType.Found)
                {
                    DbSearchResult editedTypeSearch = SearchObjType(lib, oldName);
                    if (editedTypeSearch.Found)
                    {
                        CptObjectType objType = editedTypeSearch.ObjectTypes[0];
                        objType.Name = newName;
                        _db.SaveChanges();
                        return StaticMessages.ObjectTypeNameChanged(libName, oldName, newName);

                    }
                    else
                    {
                        return StaticMessages.ObjectNotFound(libName, oldName);
                    }
                }
                else
                {
                    return StaticMessages.ObjectTypeNameUnavailable(libName, newName);
                }

            }
            else
            {
                return StaticMessages.LibraryNotFound(libName);
            }
        }
        internal ConceptDbResponse AddObjTypeToLibrary(string libName, string parentType, string newType)
        {
            DbSearchResult libSearch = SearchLibrary(libName);
            if (libSearch.Found)
            {
                CptLibrary lib = libSearch.Libraries[0];
                DbSearchResult existingType = SearchObjType(lib, newType);
                if (!existingType.Found)
                {
                    parentType = parentType == String.Empty ? "root" : parentType;
                    DbSearchResult parentObjTypeSearch = SearchObjType(lib, parentType);
                    if (parentObjTypeSearch.Found)
                    {
                        CptObjectType objType = new(newType);
                        lib.ObjectTypes.Add(objType);
                        parentObjTypeSearch.ObjectTypes[0].Children.Add(objType);
                        _db.SaveChanges();
                        return StaticMessages.ObjectTypeAddedToLibrary(libName, parentType, newType);
                    }
                    else
                    {
                        return StaticMessages.ObjectTypeNotFound(libName, parentType);
                    }
                }
                else
                {
                    return StaticMessages.ObjectTypeNameUnavailable(libName, newType);
                }
            }
            else
            {
                return StaticMessages.LibraryNotFound(libName);
            }
        }


        internal ConceptDbResponse DeleteLibrary(string libName)
        {
            DbSearchResult search = SearchLibrary(libName);
            if (search.Found)
            {
                CptLibrary result = search.Libraries[0];
                _db.Remove(result);
                _db.SaveChanges();
                return StaticMessages.LibraryDeleted(result);
            }
            else
            {
                return StaticMessages.LibraryNotFound(libName);
            }
        }
        internal ConceptDbResponse RenameLibrary(string oldName, string newName)
        {
            DbSearchResult search = SearchLibrary(oldName);
            if (search.Found)
            {
                DbSearchResult newSearch = SearchLibrary(newName);
                if (newSearch.Found)
                {
                    return StaticMessages.LibraryNameUnavailable(newName);
                }
                else
                {
                    CptLibrary lib = search.Libraries[0];
                    lib.Name = newName;
                    _db.SaveChanges();
                    return StaticMessages.LibraryNameChanged(oldName, newName);
                }
            }
            else
            {
                return StaticMessages.LibraryNotFound(oldName);
            }
        }
        internal ConceptDbResponse RetrieveScopedLibrary(string name)
        {
            DbSearchResult search = SearchLibrary(name);
            if (search.Found)
            {
                LibraryScope scoped = new(search.Libraries[0]);
                ConceptDbResponse response = StaticMessages.LibraryScopeRetrieved(name, scoped);
                return response;
            }
            else
            {
                return StaticMessages.LibraryNotFound(name);
            }
        }
        internal ConceptDbResponse ViewLibraryIndex()
        {
            List<CptLibrary> libraryList = _db.Libraries.ToList();
            ConceptDbResponse response = StaticMessages.LibraryIndexRequested;
            foreach(CptLibrary library in libraryList)
            {
                response.Values.Add(library.Name);
            }
            return response;
        }
        internal ConceptDbResponse CreateLibrary(string name)
        {
            DbSearchResult search0 = SearchLibrary(name);
            if (search0.Found)
            {
                return StaticMessages.LibraryNameUnavailable(name);
            }
            else
            {
                CptLibrary newLibrary = new();
                newLibrary.Name = name;
                _db.Libraries.Add(newLibrary);
                _db.SaveChanges();
                return StaticMessages.LibraryCreated(newLibrary.Id, name);
            }
        }
        private CptLibrary? RetrieveLibrary(string name) => _db.Libraries.Where(e => e.Name == name).FirstOrDefault();
        private DbSearchResult SearchLibrary(string name)
        {
            CptLibrary? result = RetrieveLibrary(name);
            return result == null 
                ? DbSearchResult.LibNotFound 
                : new(true, ResultId.Success, new() { result }, new());
        }
        private DbSearchResult SearchObjType(CptLibrary lib, string name)
        {
            foreach(CptObjectType objType in lib.ObjectTypes)
            {
                if(objType.Name == name)
                {
                    return new(true, ResultId.Success, new() { objType });
                }
            }
            return DbSearchResult.ObjTypeNotFound;
        }
    }
}
