using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Lisa.Excelsis.WebApi
{
    public class PatchValidator
    {
        protected void Allow(string action, object resource, Patch patch, string regex, params Action<object, Patch>[] funcs)
        {
            if (Regex.IsMatch(patch.Field.ToLower(), regex))
            {
                if (patch.Action.ToLower() == action)
                {
                    foreach (Action<object, Patch> func in funcs)
                    {
                        func(resource, patch);
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
            }
            return IsValid;
        }

        protected List<Error> Errors()
        {
            return _errors;
        }

        protected static List<Error> _errors = new List<Error>();
    }
}
