using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConceptDbLib.Entities
{
    public class CptValue
    {
        public int Id { get; set; }
        public string String { get; set; }
        public float Float { get; set; }
        public virtual List<CptObject> Objects { get; set; }
        public DateTime DateTime { get; set; }
        public CptValue()
        {
            String = String.Empty;
            Float = 0;
            DateTime = DateTime.MinValue;
            Objects = new();
        }
    }
}
