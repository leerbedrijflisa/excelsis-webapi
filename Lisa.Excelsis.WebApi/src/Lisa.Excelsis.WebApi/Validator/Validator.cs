using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Lisa.Excelsis.WebApi
{
    abstract class Validator<T>
    {
        protected int ResourceId { get; set; }

        public abstract IEnumerable<Error> ValidatePatches(int id, IEnumerable<Patch> patch);

        public abstract IEnumerable<Error> ValidatePost(T post);

        protected IEnumerable<Error> Allow<T>(Patch patch, string action, Regex regex, Action<T, object>[] validateValue = null, Action<dynamic>[] validateField = null)
        {
            var match = regex.Match(patch.Field.ToLower());
            if (match.Success)
            {   
                if (patch.Action.ToLower() == action)
                {
                    if (Regex.IsMatch(patch.Action.ToLower(), @"^(add|replace|remove)$") && patch.Value == null)
                    {
                        errors.Add(new Error(1101, new ErrorProps { Field = "Value" }));
                    }
                    else if (Regex.IsMatch(patch.Action.ToLower(), @"^(move)$") && patch.Target == null)
                    {
                        errors.Add(new Error(1101, new ErrorProps { Field = "Target" }));
                    }

                    var fieldParams = new ExpandoObject() as IDictionary<string, Object>;
                    foreach (string groupName in regex.GetGroupNames())
                    {
                        fieldParams.Add(groupName, match.Groups[groupName].Value);
                    }
                    fieldParams.Add("Field", patch.Field);
                    fieldParams.Add("Value", patch.Value?.ToString());
                    fieldParams.Add("Target", patch.Target);

                    if (!fieldParams.ContainsKey("Id"))
                    {
                        fieldParams.Add("Id", patch.Value.ToString());
                    }

                    if (validateField != null)
                    {
                        foreach (var func in validateField)
                        {
                            func(fieldParams);
                        }
                    }

                    if (validateValue != null)
                    {
                        try {
                            foreach (var func in validateValue)
                            {
                                func(patch.Value.ToObject<T>(), fieldParams);
                            }
                        }
                        catch(Exception e)
                        {
                            errors.Add(new Error(1500, new ErrorProps { Exception = e.Message }));
                        }
                    }
                    patch.IsValidated = true;
                }
                patch.IsValidField = true;
            }
            return errors;
        }

        protected IEnumerable<Error> Allow<T>(string field, dynamic value, Action<T, object> validateValue, bool optional = false)
        {
            if (value == null && !optional)
            {
                errors.Add(new Error(1101, new ErrorProps { Field = field }));
            }
            else if (value == null && optional)
            {
                return null;
            }

            var fieldParams = new ExpandoObject() as IDictionary<string, Object>;
            fieldParams.Add("Field", field);
            fieldParams.Add("Value", value.ToString());

            try
            {
                validateValue(value, fieldParams);
            }
            catch (Exception e)
            {
                errors.Add(new Error(1500, new ErrorProps { Exception = e.Message }));
            }

            return errors;
        }

        protected IEnumerable<Error> SetRemainingPatchError(IEnumerable<Patch> patches)
        {
            foreach (var patch in patches)
            {
                if (!patch.IsValidated)
                {
                    if (!patch.IsValidField)
                    {
                        errors.Add(new Error(1209, new ErrorProps { Field = patch.Field }));
                    }
                    else if (patch.IsValidField)
                    {
                        errors.Add(new Error(1303, new ErrorProps { Action = patch.Action }));
                    }
                }
            }
            return (errors.Any())? errors : null;
        }

        protected List<Error> errors = new List<Error>();
    }
}