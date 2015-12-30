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
        public string ValueRegex { get; set; }
        public string Parent { get; set; }
        public Func<object, Patch, PatchPropInfo, bool> Validate { get; set; }
        public Func<object, JToken, PatchPropInfo, bool> BuildQuery { get; set; }
        public ErrorProps ErrorInfo { get; set; }
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

    public class PatchPropInfo
    {
        public string Child { get; set; }
        public string ChildId { get; set; }
        public string Parent { get; set; }
        public string ParentId { get; set; }
        public string Property { get; set; }
        public string Target { get; set; }
        public string Regex { get; set; }
        public ErrorProps ErrorInfo { get; set; }
    }
}
