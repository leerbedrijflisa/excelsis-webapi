using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Lisa.Excelsis.WebApi
{
    public class Patch
    {
        public string Action { get; set; }
        public string Field { get; set; }
        public JToken Value { get; set; }
        public string Target { get; set; }
        internal bool IsValidated { get; set; }
        internal bool IsValidField { get; set; }
        internal IList<Error> Errors { get; set; }
    }

    public class ErrorProps
    {
        public int Code { get; set; }
        public string Type { get; set; }
        public string Field { get; set; }
        public string Value { get; set; }
        public int Count { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
        public string Permitted1 { get; set; }
        public string Permitted2 { get; set; }
        public string Permitted3 { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public string Cohort { get; set; }
        public string Crebo { get; set; }
        public string Parent { get; set; }
        public string ParentId { get; set; }
        public string SubField { get; set; }
        public string Regex { get; set; }
    }
}
