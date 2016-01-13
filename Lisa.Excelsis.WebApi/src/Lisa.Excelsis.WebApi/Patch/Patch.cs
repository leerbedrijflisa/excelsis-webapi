using System;
using System.Collections.Generic;
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

        protected List<Error> Errors()
        {
            return _errors;
        }

        public string FatalError
        {
            get
            {
                return _fatalError;
            }
        }

        protected static string _fatalError { get; set; }

        protected static List<Error> _errors = new List<Error>();
    }
}
