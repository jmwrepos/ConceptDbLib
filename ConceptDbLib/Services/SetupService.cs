using CptClientShared;
using CptClientShared.Entities;

namespace ConceptDbLib.Services
{
    internal class SetupService
    {
        private readonly CptDbProvider _dbProvider;
        private ConceptContext _db => _dbProvider.Context;
        private readonly string _secKey;
        private string confirmKey;
        public SetupService(CptDbProvider dbProvider, string secKey)
        {
            _dbProvider = dbProvider;
            _secKey = secKey;
            confirmKey = Guid.NewGuid().ToString();
        }
        private ConceptDbResponse ReqDbInitializeRequestedOk => new(ConceptDbResponseId.Success, "Request Received. Be aware that this process will result in complete loss of data. To confirm, submit a 'DbInitialize' request using the following key", new() { confirmKey });
        private static ConceptDbResponse NotAuthorized => new(ConceptDbResponseId.Error, "Request could not be authorized.", new() { string.Empty });
        
        private static ConceptDbResponse DbConnected => new(ConceptDbResponseId.Information, "Database Connected", new() { string.Empty });
        private static ConceptDbResponse DbNotConnected => new(ConceptDbResponseId.Information, "Database Not Reachable", new() { string.Empty });
        internal ConceptDbResponse RequestDbInitialize(string key)
        {
            if(key == _secKey)
            {
                confirmKey = Guid.NewGuid().ToString();
                return ReqDbInitializeRequestedOk;
            }
            else
            {
                return NotAuthorized;
            }
        }        
        internal ConceptDbResponse DbInitialize(string key)
        {
            if(key == confirmKey)
            {
                return _dbProvider.RemakeDb();
            }
            else
            {
                return NotAuthorized;
            }

        }
        
        internal ConceptDbResponse CheckDbConnection(string key)
        {
            if(key == _secKey)
            {
                if (_db.Database.CanConnect())
                {
                    return DbConnected;
                }
                else
                {
                    return DbNotConnected;
                }
            }
            else
            {
                return NotAuthorized;
            }
        }

    }
}
