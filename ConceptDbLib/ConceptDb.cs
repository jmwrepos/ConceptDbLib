using ConceptDbLib.Services;
using CptClientShared;
using Microsoft.Extensions.Logging;

namespace ConceptDbLib
{
    public class ConceptDb
    {
        private readonly ILogger<ConceptDb> _logger;
        private readonly SetupService _setupService;
        private readonly CptDbProvider _dbProvider;
        private readonly Dictionary<string, BuilderService> _builders = new();
        private readonly string _secKey = "d0b8f84c-98fe-48ca-a13e-dca610648fe5";
        private ConceptDbResponse NewBuilderSuccess(string builderId) => new(ConceptDbResponseId.Success, "New Builder Created", new() { builderId });
        private ConceptDbResponse NotAuthorized => new(ConceptDbResponseId.Error, "Request could not be authorized.", new() { string.Empty });
        private ConceptDbResponse BuilderNotFound(string builderId) => new(ConceptDbResponseId.Error, "Builder could not be found", new() { builderId });
        public ConceptDb(ILogger<ConceptDb> logger)
        {
            _logger = logger;
            _dbProvider = new();
            _setupService = new(_dbProvider, _secKey);
            _logger.Log(LogLevel.Warning, string.Format("Builder SecKey:\t {0}", _secKey));
        }

        // BUILDER METHODS //
        // BUILDER METHODS //
        // BUILDER METHODS //
        public async Task<ConceptDbResponse> UnassignPropertyAsync(string builderId, string libName, string propName, string objName) =>
            await Task.Run(() => UnassignProperty(builderId, libName, propName, objName));
        public ConceptDbResponse UnassignProperty(string builderId, string libName, string propName, string objName)
        {
            if (_builders.ContainsKey(builderId))
            {
                return _builders[builderId].UnassignProperty(libName, propName, objName);
            }
            else
            {
                return BuilderNotFound(builderId);
            }
        }
        public async Task<ConceptDbResponse> AssignPropertyAsync(string builderId, string libName, string propName, string objName) =>
            await Task.Run(() => AssignProperty(builderId, libName, propName, objName));        
        public ConceptDbResponse AssignProperty(string builderId, string libName, string propName, string objName)
        {
            if (_builders.ContainsKey(builderId))
            {
                return _builders[builderId].AssignProperty(libName, propName, objName);
            }
            else
            {
                return BuilderNotFound(builderId);
            }
        }
        public async Task<ConceptDbResponse> DeletePropertyAsync(string builderId, string libName, string propName) =>
            await Task.Run(() => DeleteProperty(builderId, libName, propName));
        public ConceptDbResponse DeleteProperty(string builderId, string libName, string propName)
        {
            if (_builders.ContainsKey(builderId))
            {
                return _builders[builderId].DeleteProperty(libName, propName);
            }
            else
            {
                return BuilderNotFound(builderId);
            }
        }
        public async Task<ConceptDbResponse> RenamePropertyAsync(string builderId, string libName, string oldName, string newName) =>
            await Task.Run(() => RenameProperty(builderId, libName, oldName, newName));
        public ConceptDbResponse RenameProperty(string builderId, string libName, string oldName, string newName)
        {
            if (_builders.ContainsKey(builderId))
            {
                return _builders[builderId].RenameProperty(libName, oldName, newName);
            }
            else
            {
                return BuilderNotFound(builderId);
            }
        }
        public async Task<ConceptDbResponse> NewPropertyAsync(string builderId, string libName, string propName) =>
            await Task.Run(() => NewProperty(builderId, libName, propName));
        public ConceptDbResponse NewProperty(string builderId, string libName, string propName)
        {
            if (_builders.ContainsKey(builderId))
            {
                return _builders[builderId].NewProperty(libName, propName);
            }
            else
            {
                return BuilderNotFound(builderId);
            }
        }
        public async Task<ConceptDbResponse> MoveObjectTypeAsync(string builderId, string libName, string objTypeName, string newParentName) =>
            await Task.Run(() => MoveObjectType(builderId, libName, objTypeName, newParentName));
        public ConceptDbResponse MoveObjectType(string builderId, string libName, string objTypeName, string newParentName)
        {
            if (_builders.ContainsKey(builderId))
            {
                return _builders[builderId].MoveObjectType(libName, objTypeName, newParentName);
            }
            else
            {
                return BuilderNotFound(builderId);
            }
        }
        public async Task<ConceptDbResponse> DeleteObjectTypeAsync(string builderId, string libName, string objTypeName) =>
            await Task.Run(() => DeleteObjectType(builderId, libName, objTypeName));
        public ConceptDbResponse DeleteObjectType(string builderId, string libName, string objTypeName)
        {
            if (_builders.ContainsKey(builderId))
            {
                return _builders[builderId].DeleteObjectType(libName, objTypeName);
            }
            else
            {
                return BuilderNotFound(builderId);
            }
        }
        public async Task<ConceptDbResponse> RenameObjectTypeAsync(string builderId, string libName, string oldName, string newName) =>
            await Task.Run(() => RenameObjectType(builderId, libName, oldName, newName));
        public ConceptDbResponse RenameObjectType(string builderId, string libName, string oldName, string newName)
        {
            if (_builders.ContainsKey(builderId))
            {
                return _builders[builderId].RenameObjectType(libName, oldName, newName);
            }
            else
            {
                return BuilderNotFound(builderId);
            }
        }
        public async Task<ConceptDbResponse> NewObjectTypeAsync(string builderId, string libName, string parentType, string newType) =>
            await Task.Run(() => NewObjectType(builderId, libName, parentType, newType));
        public ConceptDbResponse NewObjectType(string builderId, string libName, string parentType, string newType)
        {
            if (_builders.ContainsKey(builderId))
            {
                return _builders[builderId].NewObjectType(libName, parentType, newType);
            }
            else
            {
                return BuilderNotFound(builderId);
            }
        }
        public async Task<ConceptDbResponse> DeleteObjectAsync(string builderId, string libName, string objName) =>
            await Task.Run(() => DeleteObject(builderId, libName, objName));
        public ConceptDbResponse DeleteObject(string builderId, string libName, string objName)
        {
            if (_builders.ContainsKey(builderId))
            {
                return _builders[builderId].DeleteObject(libName, objName);
            }
            else
            {
                return BuilderNotFound(builderId);
            }
        }
        public async Task<ConceptDbResponse> MoveObjectAsync(string builderId, string libName, string? parentName, string childName) =>
            await Task.Run(() => MoveObject(builderId, libName, parentName, childName));
        public ConceptDbResponse MoveObject(string builderId, string libName, string? parentName, string childName)
        {
            if (_builders.ContainsKey(builderId))
            {
                return _builders[builderId].MoveObject(libName, parentName, childName);
            }
            else
            {
                return BuilderNotFound(builderId);
            }
        }
        public async Task<ConceptDbResponse> RenameObjectAsync(string builderId, string libName, string oldName, string newName) =>
            await Task.Run(() => RenameObject(builderId, libName, oldName, newName));
        public ConceptDbResponse RenameObject(string builderId, string libName, string oldName, string newName)
        {
            if (_builders.ContainsKey(builderId))
            {
                return _builders[builderId].RenameObject(libName, oldName, newName);
            }
            else
            {
                return BuilderNotFound(builderId);
            }
        }
        public async Task<ConceptDbResponse> NewObjectAsync(string builderId, string libName, string objName, string objParentName, string objTypeNames) =>
            await Task.Run(() => NewObject(builderId, libName, objName, objParentName, objTypeNames));

        public ConceptDbResponse NewObject(string builderId, string libName, string objName, string objParentName, string objTypeNames)
        {
            if (_builders.ContainsKey(builderId))
            {
                return _builders[builderId].NewObject(libName, objName, objParentName, objTypeNames);
            }
            else
            {
                return BuilderNotFound(builderId);
            }
        }
        public async Task<ConceptDbResponse> DeleteLibraryAsync(string builderId, string libName) =>
            await Task.Run(() => DeleteLibrary(builderId, libName));
        public ConceptDbResponse DeleteLibrary(string builderId, string libName)
        {
            if (_builders.ContainsKey(builderId))
            {
                return _builders[builderId].DeleteLibrary(libName);
            }
            else
            {
                return BuilderNotFound(builderId);
            }
        }
        public async Task<ConceptDbResponse> RenameLibraryAsync(string builderId, string oldName, string newName) =>
            await Task.Run(() => RenameLibrary(builderId, oldName, newName));
        public ConceptDbResponse RenameLibrary(string builderId, string oldName, string newName)
        {
            if (_builders.ContainsKey(builderId))
            {
                return _builders[builderId].RenameLibrary(oldName, newName);
            }
            else
            {
                return BuilderNotFound(builderId);
            }
        }
        public async Task<ConceptDbResponse> RetrieveScopedLibraryAsync(string builderId, string name) =>
            await Task.Run(() => RetrieveScopedLibrary(builderId, name));
        public ConceptDbResponse RetrieveScopedLibrary(string builderId, string name)
        {
            if (_builders.ContainsKey(builderId))
            {
                return _builders[builderId].RetrieveScopedLibrary(name);
            }
            else
            {
                return BuilderNotFound(builderId);
            }
        }
        public async Task<ConceptDbResponse> ViewLibraryIndexAsync(string builderId) =>
            await Task.Run(() => ViewLibraryIndex(builderId));
        public ConceptDbResponse ViewLibraryIndex(string builderId)
        {
            if (_builders.ContainsKey(builderId))
            {
                return _builders[builderId].ViewLibraryIndex();
            }
            else
            {
                return BuilderNotFound(builderId);
            }
        }
        public async Task<ConceptDbResponse> CreateLibraryAsync(string builderId, string name) =>
            await Task.Run(() => CreateLibrary(builderId, name));
        public ConceptDbResponse CreateLibrary(string builderId, string name)
        {
            if (_builders.ContainsKey(builderId))
            {
                return _builders[builderId].CreateLibrary(name);
            }
            else
            {
                return BuilderNotFound(builderId);
            }
        }
        //ADMIN METHODS
        //ADMIN METHODS
        //ADMIN METHODS

        public async Task<ConceptDbResponse> RequestDbInitializeAsync(string key) => await Task.Run(() => RequestDbInitialize(key));
        public ConceptDbResponse RequestDbInitialize(string key)
        {
            return _setupService.RequestDbInitialize(key);
        }

        public async Task<ConceptDbResponse> DbInitializeAsync(string key) => await Task.Run(() => DbInitialize(key));
        public ConceptDbResponse DbInitialize(string key)
        {
            return _setupService.DbInitialize(key);
        }





        //CREATE NEW BUILDER
        //CREATE NEW BUILDER
        //CREATE NEW BUILDER
        public async Task<ConceptDbResponse> NewBuilderAsync(string secKey) => await Task.Run(() => NewBuilder(secKey));
        public ConceptDbResponse NewBuilder(string secKey)
        {
            if (_secKey == secKey)
            {
                string bid = Guid.NewGuid().ToString();
                BuilderService bs = new(_dbProvider);
                _builders[bid] = bs;
                return NewBuilderSuccess(bid);
            }
            else
            {
                return NotAuthorized;
            }
        }
    }
}
