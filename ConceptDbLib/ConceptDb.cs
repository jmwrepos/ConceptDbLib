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
        private readonly AccountingService _acctService;
        private readonly string _secKey = "d0b8f84c-98fe-48ca-a13e-dca610648fe5";
        private ConceptDbResponse NotAuthorized => new(ConceptDbResponseId.Error, "Request could not be authorized.", new() { string.Empty });
        private ConceptDbResponse BuilderNotFound(string builderId) => new(ConceptDbResponseId.Error, "Builder could not be found", new() { builderId });
        public ConceptDb(ILogger<ConceptDb> logger)
        {
            _logger = logger;
            _dbProvider = new();
            _acctService = new(_dbProvider);
            _setupService = new(_dbProvider, _acctService, _secKey);
            _logger.Log(LogLevel.Warning, string.Format("Builder SecKey:\t {0}", _secKey));
        }
        // ACCOUNT MENTHODS
        public async Task<ConceptDbResponse> DeleteAccountAsync(string tryKey, int id) =>
            await Task.Run(() => DeleteAccount(tryKey, id));
        public ConceptDbResponse DeleteAccount(string tryKey, int id)
        {
            if (tryKey == _secKey)
            {
                return _acctService.DeleteAccount(id);
            }
            else
            {
                return NotAuthorized;
            }
        }
        public async Task<ConceptDbResponse> DeleteUserAsync(string builderId, string email) =>
            await Task.Run(() => DeleteUser(builderId, email));
        public ConceptDbResponse DeleteUser(string builderId, string email)
        {
            if (_acctService.BuilderExists(builderId))
            {
                return _acctService.GetBuilder(builderId).DeleteUser(email);
            }
            else
            {
                return BuilderNotFound(builderId);
            }
        }
        public async Task<ConceptDbResponse> InactivateAccountAsync(string tryKey, int id) =>
            await Task.Run(() => InactivateAccount(tryKey, id));
        public ConceptDbResponse InactivateAccount(string tryKey, int id)
        {
            if (tryKey == _secKey)
            {
                return _acctService.InactivateAccount(id);
            }
            else
            {
                return NotAuthorized;
            }
        }
        public async Task<ConceptDbResponse> InactivateUserAsync(string builderId, string email) =>
            await Task.Run(() => InactivateUser(builderId, email));
        public ConceptDbResponse InactivateUser(string builderId, string email)
        {
            if (_acctService.BuilderExists(builderId))
            {
                return _acctService.GetBuilder(builderId).InactivateUser(email);
            }
            else
            {
                return BuilderNotFound(builderId);
            }
        }
        public async Task<ConceptDbResponse> UpdateAccountNameAsync(string builderId, string newName) =>
            await Task.Run(() => UpdateAccountName(builderId, newName));
        public ConceptDbResponse UpdateAccountName(string builderId, string newName)
        {
            if (_acctService.BuilderExists(builderId))
            {
                return _acctService.GetBuilder(builderId).UpdateAccountName(newName);
            }
            else
            {
                return BuilderNotFound(builderId);
            }
        }
        public async Task<ConceptDbResponse> RetrieveUserScopeAsync(string builderId, string email) =>
            await Task.Run(() => RetrieveUserScope(builderId, email));
        public ConceptDbResponse RetrieveUserScope(string builderId, string email)
        {
            if (_acctService.BuilderExists(builderId))
            {
                return _acctService.GetBuilder(builderId).RetrieveUserScope(email);
            }
            else
            {
                return BuilderNotFound(builderId);
            }
        }
        public async Task<ConceptDbResponse> RetrieveAcctScopeAsync(string builderId) =>
            await Task.Run(() => RetrieveAccountScope(builderId));
        public ConceptDbResponse RetrieveAccountScope(string builderId)
        {
            if (_acctService.BuilderExists(builderId))
            {
                return _acctService.GetBuilder(builderId).RetrieveAccountScope();
            }
            else
            {
                return BuilderNotFound(builderId);
            }
        }
        public async Task<ConceptDbResponse> UpdateUserAsync(string builderId, string currentEmail, string? firstName, string? lastName, string? newEmail, string? password) =>
            await Task.Run(() => UpdateUser(builderId, currentEmail, firstName, lastName, newEmail, password));
        public ConceptDbResponse UpdateUser(string builderId, string currentEmail, string? firstName, string? lastName, string? newEmail, string? password)
        {
            if (_acctService.BuilderExists(builderId))
            {
                return _acctService.GetBuilder(builderId).UpdateUser(currentEmail, firstName, lastName, newEmail, password);
            }
            else
            {
                return BuilderNotFound(builderId);
            }
        }
        public async Task<ConceptDbResponse> NewUserAsync(string builderId, string? firstName, string? lastName, string? email, string? password) =>
            await Task.Run(() => NewUser(builderId, firstName, lastName, email, password));
        public ConceptDbResponse NewUser(string builderId, string? firstName, string? lastName, string? email, string? password)
        {
            if (_acctService.BuilderExists(builderId))
            {
                return _acctService.GetBuilder(builderId).NewUser(firstName, lastName, email, password);
            }
            else
            {
                return BuilderNotFound(builderId);
            }
        }
        public async Task<ConceptDbResponse> NewAccountAsync(string tryKey, string acctType, string acctName, string firstName, string lastName, string username, string password) =>
            await Task.Run(() => NewAccount(tryKey, acctType, acctName, firstName, lastName, username, password));
        public ConceptDbResponse NewAccount(string tryKey, string acctType, string acctName, string firstName, string lastName, string username, string password)
        {
            if(tryKey == _secKey)
            {
                return _acctService.CreateAccount(acctName, firstName, lastName, username, password, acctType);
            }
            else
            {
                return NotAuthorized;
            }
        }

        // BUILDER METHODS //
        // BUILDER METHODS //
        // BUILDER METHODS //

        public async Task<ConceptDbResponse> SetPropertyValueAsync(string builderId, string libName, string objName, string propName, string stringVals, string objNameVals, string numVals) =>
            await Task.Run(() => SetPropertyValue(builderId, libName, objName, propName, stringVals, objNameVals, numVals));
        public ConceptDbResponse SetPropertyValue(string builderId, string libName, string objName, string propName, string stringVals, string objNameVals, string numVals)
        {
            if (_acctService.BuilderExists(builderId))
            {
                return _acctService.GetBuilder(builderId).SetPropertyValue(libName, objName, propName, stringVals, objNameVals, numVals);
            }
            else
            {
                return BuilderNotFound(builderId);
            }
        }
        public async Task<ConceptDbResponse> UnassignPropertyAsync(string builderId, string libName, string propName, string objName) =>
            await Task.Run(() => UnassignProperty(builderId, libName, propName, objName));
        public ConceptDbResponse UnassignProperty(string builderId, string libName, string propName, string objName)
        {
            if (_acctService.BuilderExists(builderId))
            {
                return _acctService.GetBuilder(builderId).UnassignProperty(libName, propName, objName);
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
            if (_acctService.BuilderExists(builderId))
            {
                return _acctService.GetBuilder(builderId).AssignProperty(libName, propName, objName);
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
            if (_acctService.BuilderExists(builderId))
            {
                return _acctService.GetBuilder(builderId).DeleteProperty(libName, propName);
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
            if (_acctService.BuilderExists(builderId))
            {
                return _acctService.GetBuilder(builderId).RenameProperty(libName, oldName, newName);
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
            if (_acctService.BuilderExists(builderId))
            {
                return _acctService.GetBuilder(builderId).NewProperty(libName, propName);
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
            if (_acctService.BuilderExists(builderId))
            {
                return _acctService.GetBuilder(builderId).MoveObjectType(libName, objTypeName, newParentName);
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
            if (_acctService.BuilderExists(builderId))
            {
                return _acctService.GetBuilder(builderId).DeleteObjectType(libName, objTypeName);
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
            if (_acctService.BuilderExists(builderId))
            {
                return _acctService.GetBuilder(builderId).RenameObjectType(libName, oldName, newName);
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
            if (_acctService.BuilderExists(builderId))
            {
                return _acctService.GetBuilder(builderId).NewObjectType(libName, parentType, newType);
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
            if (_acctService.BuilderExists(builderId))
            {
                return _acctService.GetBuilder(builderId).DeleteObject(libName, objName);
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
            if (_acctService.BuilderExists(builderId))
            {
                return _acctService.GetBuilder(builderId).MoveObject(libName, parentName, childName);
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
            if (_acctService.BuilderExists(builderId))
            {
                return _acctService.GetBuilder(builderId).RenameObject(libName, oldName, newName);
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
            if (_acctService.BuilderExists(builderId))
            {
                return _acctService.GetBuilder(builderId).NewObject(libName, objName, objParentName, objTypeNames);
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
            if (_acctService.BuilderExists(builderId))
            {
                return _acctService.GetBuilder(builderId).DeleteLibrary(libName);
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
            if (_acctService.BuilderExists(builderId))
            {
                return _acctService.GetBuilder(builderId).RenameLibrary(oldName, newName);
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
            if (_acctService.BuilderExists(builderId))
            {
                return _acctService.GetBuilder(builderId).RetrieveScopedLibrary(name);
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
            if (_acctService.BuilderExists(builderId))
            {
                return _acctService.GetBuilder(builderId).ViewLibraryIndex();
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
            if (_acctService.BuilderExists(builderId))
            {
                return _acctService.GetBuilder(builderId).CreateLibrary(name);
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
        public async Task<ConceptDbResponse> NewBuilderAsync(string username, string password) => await Task.Run(() => NewBuilder(username, password));
        public ConceptDbResponse NewBuilder(string username, string password)
        {
            return _acctService.Authenticate(username, password);
        }
    }
}
