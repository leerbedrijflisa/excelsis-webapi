using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Lisa.Excelsis.WebApi
{
    partial class Validate
    {
        /*
        *
        *   EXAM VALIDATE FUNCTIONS
        *
        */
        public bool ValidateCategoryObject(object resource, Patch patch, PatchPropInfo parameters)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>()
            {
                { "order", @"^\d+$" },
                { "name", @"^[a-zA-Z\s,!?.:'""]*$" }
            };

            return ValidateObjectValues(patch, dict);
        }

        public bool ValidateCriterionObject(object resource, Patch patch, PatchPropInfo parameters)
        {
            _errors = new List<Error>();
            if (ValidateChild(resource, parameters.Child, parameters.ChildId))
            {
                Dictionary<string, string> dict = new Dictionary<string, string>()
                {
                    { "order", @"^\d+$" },
                    { "title", @"^[a-zA-Z\s,!?.:'""]*$" },
                    { "description", @"^[a-zA-Z\s,!?.:'""]*$" },
                    { "weight", @"^(fail|pass|excellent)$"}
                };

                return ValidateObjectValues(patch, dict);
            }
            else
            {
                _errors.Add(new Error(1501, new ErrorProps { Field = parameters.Child, Value = parameters.ChildId }));
            }
            return false;
        }



        public bool ValidateReplaceCategory(object resource, Patch patch, PatchPropInfo parameters)
        {
            _errors = new List<Error>();
            bool resourceExists = ValidateChild(resource, parameters.Child, parameters.ChildId);
            if (!resourceExists)
            {
                _errors.Add(new Error(1501, new ErrorProps { Field = parameters.Child, Value = parameters.ChildId }));
            }

            bool ValueIsValid = Regex.IsMatch(patch.Value.ToString(), parameters.Regex);           
            if (!ValueIsValid)
            {
                _errors.Add(new Error(parameters.ErrorInfo.Code, parameters.ErrorInfo));
            }
            return (resourceExists && ValueIsValid);
        }

        public bool ValidateReplaceCriterion(object resource, Patch patch, PatchPropInfo parameters)
        {
            _errors = new List<Error>();
            bool resourceExists = ValidateParent(resource, parameters.Parent, parameters.ParentId, parameters.Child, parameters.ChildId);
            if (!resourceExists)
            {
                _errors.Add(new Error(1502, new ErrorProps { Field = parameters.Child, Value = parameters.ChildId, Parent = parameters.Parent, ParentId = parameters.ParentId}));
            }
            bool ValueIsValid = Regex.IsMatch(patch.Value.ToString(), parameters.Regex);
            if (!ValueIsValid)
            {
                _errors.Add(new Error(parameters.ErrorInfo.Code, parameters.ErrorInfo));
            }
            return (resourceExists && ValueIsValid);
        }

        /*
        *
        *   ASSESSMENT VALIDATE FUNCTIONS
        *
        */

        public bool ValidateAssessed(object resource, Patch patch, PatchPropInfo parameters)
        {
            _errors = new List<Error>();
            DateTime temp;
            bool ValueIsValid = (DateTime.TryParse(patch.Value.ToString(), out temp));
            if (!ValueIsValid)
            {
                _errors.Add(new Error(parameters.ErrorInfo.Code, parameters.ErrorInfo));
            }
            return (ValueIsValid);
        }

        public bool ValidateMark(object resource, Patch patch, PatchPropInfo parameters)
        {
            _errors = new List<Error>();
            bool resourceExists = ValidateChild(resource, "observations", parameters.ParentId);
            if (!resourceExists)
            {
                _errors.Add(new Error(1501, new ErrorProps { Field = "observations", Value = parameters.ParentId }));
            }

            bool ValueIsValid = Regex.IsMatch(patch.Value.ToString(), parameters.Regex);
            if (!ValueIsValid)
            {
                _errors.Add(new Error(parameters.ErrorInfo.Code, parameters.ErrorInfo));
            }
            return (resourceExists && ValueIsValid);
        }

        public bool ValidateResult(object resource, Patch patch, PatchPropInfo parameters)
        {
            _errors = new List<Error>();
            bool resourceExists = ValidateChild(resource, parameters.Child, parameters.ChildId);
            if (!resourceExists)
            {
                _errors.Add(new Error(1501, new ErrorProps { Field = parameters.Child, Value = parameters.ChildId }));
            }

            bool ValueIsValid = Regex.IsMatch(patch.Value.ToString(), parameters.Regex);
            if (!ValueIsValid)
            {
                _errors.Add(new Error(parameters.ErrorInfo.Code, parameters.ErrorInfo));
            }
            return (resourceExists && ValueIsValid);
        }

        /*
         *  MISC
         */
        // check if child exist in parent and parent exist in resource
        public bool ValidateChild(dynamic resource, string child, object childId)
        {
            var query = @"SELECT COUNT(*) as count FROM " + child + @"
                          WHERE " + resource.Name + @" = " + resource.Value + @" AND Id = @Id";
            var parameters = new { id = childId };

            dynamic result = _db.Execute(query, parameters);
            return (result.count > 0);
        }
        // check if parent exist in resource
        public bool ValidateParent(dynamic resource, string parent, string parentId, string child, object childId)
        {
            var query = @"SELECT COUNT(*) as count FROM " + child + @"
                          WHERE " + resource.Name + @" = " + resource.Value + @" AND Id = @Id AND " + parent + @" = @ParentId";
            var parameters = new { Id = childId, ParentId = parentId };

            dynamic result = _db.Execute(query, parameters);
            return (result.count > 0);
        }
        //validate remove child
        public bool ValidateRemove(object resource, Patch patch, PatchPropInfo parameters)
        {
            _errors = new List<Error>();
            if (Regex.IsMatch(patch.Value.ToString(), parameters.Regex))
            {
                return ValidateParent(resource, parameters.Parent, parameters.ParentId, parameters.Child, patch.Value.ToString());
            }
            return false;
        }
        // validate remove parent
        public bool ValidateRemoveOneResource(object resource, Patch patch, PatchPropInfo parameters)
        {
            _errors = new List<Error>();
            if (Regex.IsMatch(patch.Value.ToString(), parameters.Regex))
            {
                return ValidateChild(resource, parameters.Child, patch.Value.ToString());
            }
            return false;
        }

        // validate value
        public bool ValidateValue(object resource, Patch patch, PatchPropInfo parameters)
        {
            _errors = new List<Error>();
            if (Regex.IsMatch(patch.Value.ToString(), parameters.Regex))
            {
                return true;
            }
            else
            {
                _errors.Add(new Error(parameters.ErrorInfo.Code, parameters.ErrorInfo));
                return false;
            }
        }


        // validate object values to add a new list item
        public bool ValidateObjectValues(Patch patch, Dictionary<string, string> dict)
        {
            _errors = new List<Error>();
            int Count = 0;
            string regex;
            var value = patch.Value as JObject;
            if (value != null)
            {
                foreach (var prop in value)
                {
                    if (dict.TryGetValue(prop.Key, out regex))
                    {
                        if (Regex.IsMatch(prop.Value.ToString(), regex))
                        {
                            Count++;
                            dict.Remove(prop.Key);
                        }
                        else
                        {
                            _errors.Add(new Error(0, new ErrorProps { Field = prop.Key, Regex = regex }));
                        }
                    }
                    else
                    {
                        _errors.Add(new Error(1103, new ErrorProps { Field = prop.Key }));
                    }
                }

                foreach(var prop in dict)
                {
                    _errors.Add(new Error(1102, new ErrorProps { Field = "value", Type = "object", SubField = prop.Key }));
                }
            }
            else
            {
                _errors.Add(new Error(1208, new ErrorProps { Field = "Value", Value = patch.Value.ToString(), Type = "object" }));
            }

            return (Count == dict.Count);
        }
    }
}
