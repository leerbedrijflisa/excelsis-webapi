using Lisa.Common.Sql;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Microsoft.AspNet.Mvc.ModelBinding;
using System.Linq;
using Newtonsoft.Json;

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
        public string FatalError
        {
            get
            {
                return _fatalError;
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
            var value = patch.Value as JObject;
            if (value != null)
            {
                foreach (var property in value)
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
            }
            else
            {
                _errors.Add(new Error(1208, new { field = "value", value = patch.Value, type = "object" }));
            }
            return dict;
        }

        public void FieldsExists (Dictionary<string,string> dict, List<string> fields)
        {
            foreach (var field in fields)
            {
                if (!dict.ContainsKey(field))
                {
                    _errors.Add(new Error(1102, new { subField = field, field = "value", type = "object"}));
                }
            }
        }

        public bool GetModelStateErrors(ModelStateDictionary ModelState)
        {
            bool fatalError = false;
            _errors = new List<Error>();
            var modelStateErrors = ModelState.Select(M => M).Where(X => X.Value.Errors.Count > 0);
            foreach (var property in modelStateErrors)
            {
                var propertyName = property.Key;
                foreach (var error in property.Value.Errors)
                {
                    if (error.Exception == null)
                    {
                        _errors.Add(new Error(1101, new { field = propertyName }));
                    }
                    else
                    {
                        if(Regex.IsMatch(error.Exception.Message, @"^Could not find member"))
                        {
                            string[] errorString = error.Exception.Message.Split('\'');
                            _errors.Add(new Error(1103, new { field = errorString[1] } ));
                        }
                        else
                        {
                            fatalError = true;
                            _fatalError = JsonConvert.SerializeObject(error.Exception.Message);
                        }
                    }
                }
            }
            return (fatalError);
        }

        private List<Error> _errors { get; set; }

        private string _fatalError { get; set; }

        private Gateway _gateway = new Gateway(Environment.GetEnvironmentVariable("ConnectionString"));
    }
}