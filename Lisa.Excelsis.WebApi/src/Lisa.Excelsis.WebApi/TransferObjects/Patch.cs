using Newtonsoft.Json.Linq;

namespace Lisa.Excelsis.WebApi
{
    public class Patch
    {
        public string Action { get; set; }
        public string Field { get; set; }
        public JToken Value { get; set; }
        public string Target { get; set; }
    }
}
