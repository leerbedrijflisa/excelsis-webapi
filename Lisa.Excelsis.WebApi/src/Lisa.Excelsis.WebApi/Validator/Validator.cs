using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Lisa.Excelsis.WebApi
{
    abstract class Validator<T>
    {
        protected int? ResourceId { get; set; }

        public abstract IEnumerable<Error> ValidatePatches(int id, IEnumerable<Patch> patch);

        public abstract IEnumerable<Error> ValidatePost(T post);

        protected Error Allow<T>(Patch patch, string action, string pattern, Func<dynamic, Error> validateField = null , Func<T, Error> validateValue = null)
        {
            if (Regex.IsMatch(patch.Field.ToLower(), pattern))
            {
                if (patch.Action.ToLower() == action)
                {
                    if (validateField != null)
                    {
                        var field = patch.Field.Split('/');
                        Dictionary<string, string> fieldParams = new Dictionary<string, string>();
                        fieldParams["parent"] = field[1] ?? patch.Value.ToString();
                        fieldParams["child"] = field[3] ?? patch.Value.ToString();
                        return validateField(fieldParams);
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

        protected IEnumerable<Error> SetRemainingPatchError(IEnumerable<Patch> patches)
        {
            List<Error> errors = new List<Error>();

            foreach (var patch in patches)
            {
                if (!patch.IsValidField)
                {
                    errors.Add(new Error(0, new ErrorProps { }));
                }
                else if (patch.IsValidField && !patch.IsValidated)
                {
                    errors.Add(new Error(0, new ErrorProps { }));
                }
            }
            return (errors.Any())? errors : null;            
        }
    }
}
