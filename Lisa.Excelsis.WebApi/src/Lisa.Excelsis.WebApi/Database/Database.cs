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
                        _errors.Add(new Error(1205, new { field = property.Key, value = property.Value.ToString() }));
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
            string fatalErrorMessage = string.Empty;
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
                        fatalError = true;
                        _fatalError = JsonConvert.SerializeObject(error.Exception.Message);
                    }
                }
            }
            return (fatalError);
        }

        public bool CheckResource(dynamic resource, string child, object childId)
        {
            var query = @"SELECT COUNT(*) as count FROM " + child + @"
                          WHERE " + resource.Name + @" = " + resource.Value + @" AND Id = @Id";
            dynamic result = _gateway.SelectSingle(query, new { Id = childId });
            return (result.count > 0);
        }

        public bool CheckResourceInResource(dynamic resource, string parent, string parentId, string child, object childId)
        {
            var query = @"SELECT COUNT(*) as count FROM " + child + @"
                          WHERE " + resource.Name + @" = " + resource.Value + @" AND Id = @Id AND " + parent + @" = @ParentId";
            dynamic result = _gateway.SelectSingle(query, new { Id = childId, ParentId = parentId });
            return (result.count > 0);
        }

        private List<PatchValidation> GetExamPatchValidation()
        {
            /*
             *  FieldRegex is for validation of the json field 'field', this will be used to create a query.
             *  Parent defines the name of the database field to check if the parent exists.
             */
            List<PatchValidation> pVal = new List<PatchValidation>();
            Validate val = new Validate();

            pVal.Add(new PatchValidation
            {
                Action = "add",
                properties = new List<PatchValidationProps>
                    {
                        new PatchValidationProps { FieldRegex = @"^categories$"},
                        new PatchValidationProps { FieldRegex = @"^(?<child>categories)/(?<childId>\d+)/criteria$"}
                    }
            });
            pVal.Add(new PatchValidation
            {
                Action = "remove",
                properties = new List<PatchValidationProps>
                    {
                        new PatchValidationProps { FieldRegex = @"^categories$"},
                        new PatchValidationProps { FieldRegex = @"^categories/(\d+)/criteria$"}
                    }
            });
            pVal.Add(new PatchValidation
            {
                Action = "replace",
                properties = new List<PatchValidationProps>
                    {
                        new PatchValidationProps { FieldRegex = @"^(?<child>categories)/(?<childId>\d+)/order$", Validate = val.ValidateCategoryOrder },
                        new PatchValidationProps { FieldRegex = @"^(?<child>categories)/(?<childId>\d+)/name$", Validate = val.ValidateCategoryName },
                        new PatchValidationProps { FieldRegex = @"^(?<parent>categories)/(?<parentId>\d+)/(?<child>criteria)/(?<childId>\d+)/order$", Parent = "CategoryId", Validate = val.ValidateCriterionOrder },
                        new PatchValidationProps { FieldRegex = @"^(?<parent>categories)/(?<parentId>\d+)/(?<child>criteria)/(?<childId>\d+)/title$", Parent = "CategoryId", Validate = val.ValidateCriterionTitle },
                        new PatchValidationProps { FieldRegex = @"^(?<parent>categories)/(?<parentId>\d+)/(?<child>criteria)/(?<childId>\d+)/description$", Parent = "CategoryId", Validate = val.ValidateCriterionDescription },
                        new PatchValidationProps { FieldRegex = @"^(?<parent>categories)/(?<parentId>\d+)/(?<child>criteria)/(?<childId>\d+)/weight$", Parent = "CategoryId", Validate = val.ValidateCriterionWeight }
                    }
            });
            pVal.Add(new PatchValidation
            {
                Action = "move",
                properties = new List<PatchValidationProps>
                    {
                        new PatchValidationProps { FieldRegex = @"^categories/(\d+)/criteria/(\d+)$"}
                    }
            });
            return pVal;
        }

        private List<Error> _errors { get; set; }

        private string _fatalError { get; set; }

        private Gateway _gateway = new Gateway(Environment.GetEnvironmentVariable("ConnectionString"));
    }
}