using Lisa.Common.Sql;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace Lisa.Excelsis.WebApi
{
    partial class Database : IDisposable
    {
        public void Dispose()
        {
            _gateway?.Dispose();
        }

        public IEnumerable<Error> Errors
        {
            get
            {
                return _errors;
            }
        }

        public string CleanParam(string name)
        {
            List<string> nameParts = new List<string>();
            Regex regex = new Regex(@"[\w\d\.]+");
            var matches = regex.Matches(name.ToLower());
            foreach(Match match in matches)
            {
                nameParts.Add(match.Value);
            }
            return string.Join("-", nameParts);
        }

        public Dictionary<string, string> IsPatchable (Patch patch, List<string> fields, string regex)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var property in (JObject)patch.Value)
            {
                if (Regex.IsMatch(property.Key.ToLower(), regex))
                {
                    dict.Add(property.Key.ToLower(), property.Value.ToString());
                }
                else
                {
                    _errors.Add(new Error(1205, new { field = property.Key }));
                }
            }
            return dict;
        }

        public void FieldsExists (Dictionary<string,string> dict, List<string> fields)
        {
            foreach (var field in fields)
            {
                if (!dict.ContainsKey(field))
                {
                    _errors.Add(new Error(1101, new { field = field }));
                }
            }
        }

        private List<Error> _errors { get; set; }

        private Gateway _gateway = new Gateway(Environment.GetEnvironmentVariable("ConnectionString"));
    }
}