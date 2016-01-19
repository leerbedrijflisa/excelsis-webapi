using System;
using System.Collections.Generic;
using System.Dynamic;
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
                patch.IsValidField = true;

                if (patch.Action.ToLower() == action)
                {
                    patch.IsValidated = true;

                    if (Regex.IsMatch(patch.Action.ToLower(), @"^(add|replace|remove)$") && patch.Value == null)
                    {
                        return new Error(9, new ErrorProps { });
                    }
                    else if (Regex.IsMatch(patch.Action.ToLower(), @"^(move)$") && patch.Target == null)
                    {
                        return new Error(9, new ErrorProps { });
                    }

                    var fieldParams = new ExpandoObject() as IDictionary<string, Object>;
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
                        try {
                            return validateValue(patch.Value.ToObject<T>(), fieldParams);
                        }
                        catch(Exception e)
                        {
                            return new Error(0, new ErrorProps { });
                        }
                    }
                }                     
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

            var fieldParams = new ExpandoObject() as IDictionary<string, Object>;
            fieldParams.Add("Field", field);
            fieldParams.Add("Value", value.ToString());

            try
            {
                return validateValue(value.ToObject<T>(), fieldParams);
            }
            catch (Exception e)
            {
                return new Error(0, new ErrorProps { });
            }
        }

        protected IEnumerable<Error> SetRemainingPatchError(IEnumerable<Patch> patches)
        {
            List<Error> errors = new List<Error>();

            foreach (var patch in patches)
            {
                if (!patch.IsValidated)
                {
                    if (!patch.IsValidField)
                    {
                        errors.Add(new Error(1500, new ErrorProps { Field = patch.Field }));
                    }
                    else if (patch.IsValidField)
                    {
                        errors.Add(new Error(1303, new ErrorProps { Value = patch.Action }));
                    }
                }
            }
            return (errors.Any())? errors : null;            
        }
    }
}
