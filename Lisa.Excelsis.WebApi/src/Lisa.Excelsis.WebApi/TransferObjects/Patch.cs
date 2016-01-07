using Newtonsoft.Json.Linq;

namespace Lisa.Excelsis.WebApi
{
    public class QueryData
    {
        public string Query { get; set; }
        public object Parameters { get; set; }
    }

    public class Patch
    {
        public string Action { get; set; }
        public string Field { get; set; }
        public JToken Value { get; set; }
        public string Target { get; set; }
        internal bool IsValidated { get; set; }
        internal bool IsValidField { get; set; }
    }
}