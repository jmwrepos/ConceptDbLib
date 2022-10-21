using CptClientShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConceptDbLib.Services
{
    public class CptDbProvider
    {
        private ConceptContext db;
        public ConceptContext Context => db;
        private static ConceptDbResponse DbInitializeOk => new(ConceptDbResponseId.Success, "Database initialization complete.", new() { string.Empty });
        public CptDbProvider()
        {
            db = NewContext;
        }
        public ConceptDbResponse RemakeDb()
        {

            Context.DetachAllEntities();
            Context.Database.EnsureDeleted();
            Context.Database.EnsureCreated();
            Context.SaveChanges();
            Context.Dispose();
            db = NewContext;
            return DbInitializeOk;
        }

        private static ConceptContext NewContext => new();
    }
}
