using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConceptDbLib.Entities
{
    public class CptNetwork
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual List<CptObject> Objects { get; set; }
        public virtual List<CptNetworkNode> Nodes { get; set; }
        public CptNetwork(string name)
        {
            Name = name;
            Objects = new();
            Nodes = new();
        }
    }
}
