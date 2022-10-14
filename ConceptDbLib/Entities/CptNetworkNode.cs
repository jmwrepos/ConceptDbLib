using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConceptDbLib.Entities
{
    public class CptNetworkNode
    {
        public int Id { get; set; }
        internal string ObjectName => CptObject.Name;
        public string Name { get; set; }
        public virtual CptObject CptObject { get; set; }
        public virtual List<CptNetworkNode> ParentNodes { get; set; }
        public virtual List<CptNetworkNode> ChildNodes { get; set; }
        public virtual List<CptValue> Values { get; set; }
        public virtual CptNetwork Network { get; set; }

        public CptNetworkNode(CptObject cptObject, string name)
        {
            CptObject = cptObject;
            ParentNodes = new();
            ChildNodes = new();
            Values = new();
            Network = null!;
            Name = name;
        }
        public CptNetworkNode()
        {
            CptObject = new(string.Empty);
            ParentNodes = new();
            ChildNodes = new();
            Values = new();
            Network = null!;
            Name = String.Empty;
        }
    }
}
