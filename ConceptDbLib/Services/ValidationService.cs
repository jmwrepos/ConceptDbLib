using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConceptDbLib.Services
{
    internal class ValidationService
    {
        private readonly CptDbProvider _dbProvider;
        private ConceptContext _db => _dbProvider.Context;
        public ValidationService(CptDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }
        public bool UsernameAvailable(string username) => !_db.AccountUsers.Any(e => e.Email.ToLower() == username.ToLower());

    }
}
