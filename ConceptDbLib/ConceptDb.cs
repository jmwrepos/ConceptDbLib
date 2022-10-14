using ConceptDbLib.Services;
using Microsoft.Extensions.Logging;

namespace ConceptDbLib
{
    public class ConceptDb : IConceptDb
    {
        private readonly ILogger<ConceptDb> _logger;
        private readonly ConceptContext _db;
        private readonly SetupService _setupService;
        private readonly Dictionary<string, BuilderService> _builders = new();
        private readonly string _secKey = Guid.NewGuid().ToString();
        private ConceptDbResponse NewBuilderSuccess(string builderId) => new(ConceptDbResponseId.Success, "New Builder Created", new() { builderId });
        private ConceptDbResponse NotAuthorized => new(ConceptDbResponseId.Error, "Request could not be authorized.", new() { string.Empty });
        private ConceptDbResponse BuilderNotFound(string builderId) => new(ConceptDbResponseId.Error, "Builder could not be found", new() { builderId });
        public ConceptDb(ILogger<ConceptDb> logger)
        {
            _logger = logger;
            _db = new();
            _setupService = new(_db, _secKey);
            _logger.Log(LogLevel.Warning, string.Format("Builder SecKey:\t {0}", _secKey));
        }

        // BUILDER METHODS //
        // BUILDER METHODS //
        // BUILDER METHODS //
        public async Task<ConceptDbResponse> CreateNetworkAsync(string builderId, string name) => await Task.Run(() => CreateNetwork(builderId, name));
        public ConceptDbResponse CreateNetwork(string builderId, string name)
        {
            bool builder = BuilderExists(builderId);
            if (builder)
            {
                BuilderService bs = _builders[builderId];
                return bs.CreateNetwork(name);
            }
            else
            {
                return BuilderNotFound(builderId);
            }
        }
        public async Task<ConceptDbResponse> CreateObjectLibraryAsync(string builderId, string name) => await Task.Run(() => CreateObjectLibrary(builderId, name));
        public ConceptDbResponse CreateObjectLibrary(string builderId, string name)
        {
            bool builder = BuilderExists(builderId);
            if (builder)
            {
                BuilderService bs = _builders[builderId];
                return bs.CreateObjectLibrary(name);
            }
            else
            {
                return BuilderNotFound(builderId);
            }
        }

        public async Task<ConceptDbResponse> CreateObjectAsync(string builderId, string name) => await Task.Run(() => CreateObject(builderId, name));
        public ConceptDbResponse CreateObject(string builderId, string name)
        {
            bool builder = BuilderExists(builderId);
            if (builder)
            {
                BuilderService bs = _builders[builderId];
                return bs.CreateObject(name);
            }
            else
            {
                return BuilderNotFound(builderId);
            }
        }
        public async Task<ConceptDbResponse> AddObjectToLibraryAsync(string builderId, string objName, string libName) => await Task.Run(() => AddObjectToLibrary(builderId, objName, libName));
        public ConceptDbResponse AddObjectToLibrary(string builderId, string objName, string libName)
        {
            bool builder = BuilderExists(builderId);
            if (builder)
            {
                BuilderService bs = _builders[builderId];
                return bs.AddObjectToLibrary(objName, libName);
            }
            else
            {
                return BuilderNotFound(builderId);
            }
        }

        public async Task<ConceptDbResponse> AddObjectToNetworkAsync(string builderId, string objName, string networkName) => await Task.Run(() => AddObjectToNetwork(builderId, objName, networkName));
        public ConceptDbResponse AddObjectToNetwork(string builderId, string objName, string networkName)
        {
            bool builder = BuilderExists(builderId);
            if (builder)
            {
                BuilderService bs = _builders[builderId];
                return bs.AddObjectToNetwork(objName, networkName);
            }
            else
            {
                return BuilderNotFound(builderId);
            }
        }

        public async Task<ConceptDbResponse> CreateNetworkMembershipAsync(string builderId, string networkName, string parentObjName, string childObjName) => await Task.Run(() => CreateNetworkMembership(builderId, networkName, parentObjName, childObjName));
        public ConceptDbResponse CreateNetworkMembership(string builderId, string networkName, string parentObjName, string childObjName)
        {
            bool builder = BuilderExists(builderId);
            if (builder)
            {
                BuilderService bs = _builders[builderId];
                return bs.CreateNetworkMembership(networkName, parentObjName, childObjName);
            }
            else
            {
                return BuilderNotFound(builderId);
            }
        }
        //ADMIN METHODS
        //ADMIN METHODS
        //ADMIN METHODS

        public async Task<ConceptDbResponse> CheckDbConnectionAsync(string key) => await Task.Run(() => CheckDbConnection(key));
        public ConceptDbResponse CheckDbConnection(string key)
        {
            return _setupService.CheckDbConnection(key);
        }


        public async Task<ConceptDbResponse> DbInitializeAsync(string key) => await Task.Run(() => DbInitialize(key));
        public ConceptDbResponse DbInitialize(string key)
        {
            return _setupService.DbInitialize(key);
        }


        public async Task<ConceptDbResponse> RequestDbInitializeAsync(string key) => await Task.Run(() => RequestDbInitialize(key));
        public ConceptDbResponse RequestDbInitialize(string key)
        {
            return _setupService.RequestDbInitialize(key);
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
                BuilderService bs = new(_db);
                _builders[bid] = bs;
                return NewBuilderSuccess(bid);
            }
            else
            {
                return NotAuthorized;
            }
        }

        private bool BuilderExists(string builderId)
        {
            return _builders.ContainsKey(builderId);
        }
    }
}
