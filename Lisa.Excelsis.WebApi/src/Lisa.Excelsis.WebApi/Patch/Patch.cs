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
    }
    public class PatchValidation
    {
        public string Action { get; set; }
        public List<PatchValidationProps> properties { get; set; }
    }
    public class PatchValidationProps
    {
        public string FieldRegex { get; set; }
        public string Parent { get; set; }
        public Func<object, Patch, object, bool> Validate { get; set; }
        public Func<object, JToken, bool> BuildQuery { get; set; }
    }
}
