using Newtonsoft.Json.Linq;
using System;
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
        internal IList<Error> Errors { get; set; }
    }
}
