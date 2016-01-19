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

        protected Error Allow<T>(Patch patch, string action, Regex regex, Func<T, object, Error> validateValue, params Func<dynamic, Error>[] validateField)
        {
            var match = regex.Match(patch.Field.ToLower());
            if (match.Success)
            {
                if (patch.Action.ToLower() == action)
                {
                    if(Regex.IsMatch(patch.Action.ToLower(), @"^(add|replace|remove)$") && patch.Value == null)
                    {
                        return new Error(9, new ErrorProps { });
                    }
                    else if (Regex.IsMatch(patch.Action.ToLower(), @"^(move)$") && patch.Target == null)
                    {
                        return new Error(9, new ErrorProps { });
                    }

                    Dictionary<string, string> fieldParams = new Dictionary<string, string>();
                    foreach (string groupName in regex.GetGroupNames())
                    {
                        fieldParams.Add(groupName, match.Groups[groupName].Value);
                    }
                    fieldParams.Add("Field", patch.Field);
                    fieldParams.Add("Value", patch.Value.ToString());
                    fieldParams.Add("Target", patch.Target);
                    
                    if (validateField != null)
                    {                       
                        foreach (var func in validateField)
                        {
                            var error = func(fieldParams);
                            if(error != null)
                            {
                                return error;
                            }
                        }
                    }

                    if (validateValue != null)
                    {
                       return validateValue(patch.Value.ToObject<T>(), fieldParams);
                    }

                    patch.IsValidated = true;
                }
                patch.IsValidField = true;          
            }
            return null;
        }

        protected Error Allow<T>(string field, dynamic value, Func<T, object, Error> validateValue, bool optional = false)
        {
            if (value == null && !optional)
            {
                return new Error(1101, new ErrorProps { Field = field });
            }
            else if( value == null && optional)
            {
                return null;
            }

            Dictionary<string, string> fieldParams = new Dictionary<string, string>();
            fieldParams.Add("Field", field);
            fieldParams.Add("Value", value.ToString());

            return validateValue(value.ToObject<T>(), fieldParams);
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
