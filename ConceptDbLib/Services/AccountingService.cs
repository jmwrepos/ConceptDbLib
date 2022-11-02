using CptClientShared;
using CptClientShared.Entities.Accounting;

namespace ConceptDbLib.Services
{
    internal class AccountingService
    {
        private readonly CptDbProvider _dbProvider;
        private ConceptContext _db => _dbProvider.Context;
        private readonly Dictionary<string, BuilderService> _builders = new();
        public AccountingService(CptDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public bool BuilderExists(string id) => _builders.ContainsKey(id);
        public BuilderService GetBuilder(string id) => _builders[id];

        public ConceptDbResponse DeleteAccount(int id)
        {
            bool exists = _db.Accounts.Any(x => x.Id == id);
            if (exists)
            {
                CptAccount acct = _db.Accounts.Find(id)!;
                _db.Accounts.Remove(acct);
                _db.SaveChanges();
                return new(ConceptDbResponseId.Success, "Account Deleted. [id]", new() { id.ToString() });
            }
            else
            {
                return new(ConceptDbResponseId.Error, "Account Not Found [id]", new() { id.ToString() });
            }
        }
        public ConceptDbResponse InactivateAccount(int id)
        {
            bool exists = _db.Accounts.Any(x => x.Id == id);
            if (exists)
            {
                CptAccount acct = _db.Accounts.Find(id)!;
                acct.Active = false;
                _db.SaveChanges();
                return new(ConceptDbResponseId.Success, "Account Inactivated. [id]", new() { acct.Id.ToString() });
            }
            else
            {
                return new(ConceptDbResponseId.Error, "Account Not Found [id]", new() { id.ToString() });
            }
        }
        public ConceptDbResponse Authenticate(string email, string tryPassword)
        {
            bool userExists = _db.AccountUsers.Any(e => e.Email.ToLower() == email.ToLower());            
            if (userExists)
            {                
                CptAcctUser user = _db.AccountUsers.Where(e => e.Email.ToLower() == email.ToLower()).First();
                bool active = user.Active && user.Account.Active;
                if (active)
                {
                    byte[] key = user.Account.EncryptionKey;
                    byte[] iv = user.UserIV;
                    string pw = ApiEncryption.Decrypt(key, iv, user.Password);
                    if (pw == tryPassword)
                    {
                        BuilderService builderSession = new(_dbProvider, user);
                        _builders.Add(builderSession.Id, builderSession);
                        return new(ConceptDbResponseId.Success, "Authentication Successful: [SessionId]", new() { builderSession.Id });
                    }
                    else
                    {
                        return new(ConceptDbResponseId.Error, "Authentication Failed: [user/email]", new() { email });
                    }
                }
                else
                {
                    return new(ConceptDbResponseId.Error, "Inactive User or Account: [email]", new() { email });
                }
            }
            else
            {
                return new(ConceptDbResponseId.Error, "User Not Found", new() { email });
            }
        }
        public ConceptDbResponse CreateAccountType(string type)
        {
            bool exists = _db.AccountTypes.Any(e => e.Name == type);
            if (!exists)
            {
                CptAcctType newType = new()
                {
                    Name = type,
                    Active = true
                };
                _db.AccountTypes.Add(newType);
                _db.SaveChanges();
                return new(ConceptDbResponseId.Success, "Account Type Created: [type]", new() { type });
            }
            else
            {
                return new(ConceptDbResponseId.Error, "Account Type Already Exists: [type]", new() { type });
            }
        }
        public ConceptDbResponse CreateAccount(string acctName, string firstName, string lastName, string email, string password, string acctType)
        {
            bool acctTypeExists = _db.AccountTypes.Any(e => e.Name == acctType);
            if (acctTypeExists)
            {
                try
                {
                    CptAcctType t = _db.AccountTypes.Where(e => e.Name == acctType).First();
                    CptAccount newAcct = new()
                    {
                        AccountName = acctName,
                        EncryptionKey = ApiEncryption.NewKey(),
                        Active = true
                    };
                    CptAcctUser newUser = new()
                    {
                        FirstName = firstName,
                        LastName = lastName,
                        Email = email,
                        UserIV = ApiEncryption.NewIV(),
                        Active = true
                    };
                    newUser.Password = ApiEncryption.Encrypt(newAcct.EncryptionKey, newUser.UserIV, password);
                    newAcct.Users.Add(newUser);
                    t.Accounts.Add(newAcct);
                    _db.SaveChanges();
                    return new(ConceptDbResponseId.Success, "Account Created: [acctName]", new() { acctName });
                }
                catch (Exception e)
                {
                    return new(ConceptDbResponseId.Error, "Database Error: [exception]", new() { e.ToString() });
                }
            }
            else
            {
                return new(ConceptDbResponseId.Error, "Account Type Not Found", new() { acctType });
            }
        }
    }
}
