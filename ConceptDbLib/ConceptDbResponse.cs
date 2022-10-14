using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConceptDbLib
{
    public class ConceptDbResponse
    {
        public ConceptDbResponseId Rid { get; set; }
        public string Message { get; set; }
        public List<string> Values { get; set; }
        public ConceptDbResponse(ConceptDbResponseId rid, string message, List<string> values)
        {
            Rid = rid;
            Message = message;
            Values = values;
        }
    }
    public enum ConceptDbResponseId
    {
        Unspecified,
        Success,
        Error,
        Information
    }
}
