using ConceptDbLib.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConceptDbLib.Services
{
    internal class SetupService
    {
        private readonly ConceptContext _db;
        private readonly string _secKey;
        private string confirmKey;
        public SetupService(ConceptContext db, string secKey)
        {
            _db = db;
            _secKey = secKey;
            confirmKey = Guid.NewGuid().ToString();
        }
        private ConceptDbResponse ReqDbInitializeRequestedOk => new(ConceptDbResponseId.Information, "Request Received. Be aware that this process will result in complete loss of data. To confirm, submit a 'DbInitialize' request using the following key", new() { confirmKey });
        private ConceptDbResponse NotAuthorized => new(ConceptDbResponseId.Error, "Request could not be authorized.", new() { string.Empty });
        private ConceptDbResponse DbInitializeOk => new(ConceptDbResponseId.Success, "Database initialization complete.", new() { string.Empty });
        private ConceptDbResponse DbConnected => new(ConceptDbResponseId.Information, "Database Connected", new() { string.Empty });
        private ConceptDbResponse DbNotConnected => new(ConceptDbResponseId.Information, "Database Not Reachable", new() { string.Empty });
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
                _db.Database.EnsureDeleted();
                _db.Database.EnsureCreated();
                return DbInitializeOk;
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
