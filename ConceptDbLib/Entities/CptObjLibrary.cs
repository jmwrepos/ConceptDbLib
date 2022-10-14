using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConceptDbLib.Entities
{
    public class CptObjLibrary
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual List<CptObject> Objects { get; set; }
        public CptObjLibrary()
        {
            Name = string.Empty;
            Objects = new();
        }
        public CptObjLibrary(string name)
        {
            Name = name;
            Objects = new();
        }
    }
}
