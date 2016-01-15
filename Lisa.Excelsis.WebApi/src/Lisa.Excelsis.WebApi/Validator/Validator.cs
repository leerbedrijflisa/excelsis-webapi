using Microsoft.AspNet.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Lisa.Excelsis.WebApi
{
    abstract class Validator<T>
    {
        public abstract IEnumerable<Error> ValidatePatch(Patch patch);

        public abstract IEnumerable<Error> ValidatePost(T post);

        protected Error Allow<T>(Patch patch, string action, string pattern, Func<object, Error> validateField = null , Func<T, Error> validateValue = null)
        {
            if (Regex.IsMatch(patch.Field.ToLower(), pattern))
            {
                if (patch.Action.ToLower() == action)
                {
                    if (validateField != null)
                    {
                        var field = patch.Field.Split('/');
                        Dictionary<string, string> fieldParams = new Dictionary<string, string>();
                        fieldParams["resource"] = "";
                        fieldParams["parent"] = field[1];
                        fieldParams["child"] = field[3];
                        return validateField(fieldParams));
                    }

                    if (validateValue != null)
                    {
                       return validateValue(patch.Value.ToObject<T>());
                    }

                    patch.IsValidated = true;
                }
                patch.IsValidField = true;          
            }
            return null;
        }

        protected Error Allow<T>(dynamic value, Func<T, Error> validateValue)
        {
            return validateValue(value.ToObject<T>());
        }

        protected Error SetRemainingPatchError(Patch patch)
        {         
            if(!patch.IsValidField)
            {
                return new Error(0, new ErrorProps { });
            }
            else if (patch.IsValidField && !patch.IsValidated)
            {
                return new Error(0, new ErrorProps { });
            }
            return null;            
        }

        public bool IsPatchValid()
        {
            if (_fatalError.Length > 0)
            {
                PatchErrors = new BadRequestObjectResult(JsonConvert.SerializeObject(_fatalError));
            }
            else if (_errors.Any())
            {
                PatchErrors = new UnprocessableEntityObjectResult(_errors);
            }

            return (_fatalError.Length == 0 && !_errors.Any());
        }

        public bool IsPostValid()
        {
            return !_errors.Any();
        }

        public IActionResult PatchErrors { get; private set; }

        public IActionResult PostErrors { get; private set; }

        protected static string _fatalError { get; set; }

        private List<Error> _errors = new List<Error>();
    }
}
