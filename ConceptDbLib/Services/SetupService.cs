using CptClientShared;

namespace ConceptDbLib.Services
{
    internal class SetupService
    {
        private readonly CptDbProvider _dbProvider;
        private ConceptContext _db => _dbProvider.Context;
        private readonly string _secKey;
        private readonly AccountingService _acctService;
        private string confirmKey;
        public SetupService(CptDbProvider dbProvider, AccountingService acctService, string secKey)
        {
            _dbProvider = dbProvider;
            _acctService = acctService;
            _secKey = secKey;
            confirmKey = Guid.NewGuid().ToString();
        }
        private ConceptDbResponse ReqDbInitializeRequestedOk => new(ConceptDbResponseId.Success, "Request Received. Be aware that this process will result in complete loss of data. To confirm, submit a 'DbInitialize' request using the following key", new() { confirmKey });
        private static ConceptDbResponse NotAuthorized => new(ConceptDbResponseId.Error, "Request could not be authorized.", new() { string.Empty });
        
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
                ConceptDbResponse r1 = _dbProvider.RemakeDb();
                if(r1.Rid == ConceptDbResponseId.Error)
                {
                    return r1;
                }
                ConceptDbResponse r2 = EnsureAccountTypes();
                if(r2.Rid == ConceptDbResponseId.Error)
                {
                    return r2;
                }
                ConceptDbResponse r3 = EnsureDefaultAccount();
                if(r3.Rid == ConceptDbResponseId.Error)
                {
                    return r3;
                }
                return new(ConceptDbResponseId.Success, "Database Initialization Complete.", new() { });
            }
            else
            {
                return NotAuthorized;
            }
        }

        private ConceptDbResponse EnsureAccountTypes()
        {
            List<string> acctTypes = new() { "Personal", "Business", "Default" };
            foreach(string acctType in acctTypes)
            {
                ConceptDbResponse response = _acctService.CreateAccountType(acctType);
                if(response.Rid != ConceptDbResponseId.Success)
                {
                    return response;
                }
            }
            return new(ConceptDbResponseId.Success, "Account Types Confirmed", new() { });
        }
        private ConceptDbResponse EnsureDefaultAccount()
        {
            bool defaultAcctExists = _db.Accounts.Any(e => e.AccountType.Name == "Default");
            if (!defaultAcctExists)
            {
                string acctName = "Default Account";
                string firstName = "Default";
                string lastName = "User";
                string email = "default@host.com";
                string password = "S*perS3cure";
                string acctType = "Default";
                return _acctService.CreateAccount(acctName, firstName, lastName, email, password, acctType);
            }
            else
            {
                return new(ConceptDbResponseId.Success, "Default Account Confirmed", new() { });
            }
        }

    }
}
