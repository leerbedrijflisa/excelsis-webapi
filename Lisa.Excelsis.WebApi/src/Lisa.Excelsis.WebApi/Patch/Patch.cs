using Microsoft.AspNet.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Lisa.Excelsis.WebApi
{
    public class PatchValidator
    {
        public static List<Error> DataAnnotationErrors = new List<Error>();

        protected void Allow(string action, int id, Patch patch, string pattern, params Action<int, Patch>[] funcs)
        {
            if (Regex.IsMatch(patch.Field.ToLower(), pattern))
            {
                if (patch.Action.ToLower() == action)
                {
                    foreach (Action<int, Patch> func in funcs)
                    {
                        func(id, patch);
                    }
                    patch.IsValidated = true;
                }
                patch.IsValidField = true;          
            }
        }

        protected bool IsValid(IEnumerable<Patch> patches)
        {
            bool IsValid = true;
            foreach (Patch patch in patches)
            {               
                if(!patch.IsValidField)
                {
                    IsValid = false;
                    _errors.Add(new Error(0, new ErrorProps { }));
                }
                else if (patch.IsValidField && !patch.IsValidated)
                {
                    IsValid = false;
                    _errors.Add(new Error(0, new ErrorProps { }));
                }
                else if( _errors.Count > 0)
                {
                    IsValid = false;
                }
            }
            return IsValid;
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
        public IActionResult PatchErrors { get; private set; }

        protected static string _fatalError { get; set; }

        protected static List<Error> _errors = new List<Error>();
    }
}
