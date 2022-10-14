using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConceptDbLib.Entities
{
    public class CptObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual List<CptObjLibrary> Libraries { get; set; }
        public virtual List<CptNetworkNode> Nodes { get; set; }
        public virtual List<CptValue> CptValues { get; set; }
        public virtual List<CptNetwork> Networks { get; set; }
        public CptObject(string name)
        {
            Name = name;
            Libraries = new();
            CptValues = new();
            Nodes = new();
            Networks = new();
        }
    }
}
