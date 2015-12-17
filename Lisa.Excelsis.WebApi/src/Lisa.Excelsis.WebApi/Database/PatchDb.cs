using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Lisa.Excelsis.WebApi
{
    partial class Database
    {
        public void Patch(object resource, IEnumerable<Patch> patches, IEnumerable<PatchValidation> validations)
        {
            _errors = new List<Error>();
            foreach (Patch patch in patches)
            {
                ValidatePatch(resource, patch, validations);
            }
        }

        private void ValidatePatch(object resource, Patch patch, IEnumerable<PatchValidation> validations)
        {
            if (patch.Field == null)
            {
                _errors.Add(new Error(1101, new { field = "field" }));
            }
            if (patch.Action == null)
            {
                _errors.Add(new Error(1101, new { field = "action" }));
            }
            if (patch.Value == null)
            {
                _errors.Add(new Error(1101, new { field = "value" }));
            }

            if (!_errors.Any())
            {
                bool foundMatch = false;
                var validation = validations.Where(v => v.Action == patch.Action.ToLower()).FirstOrDefault();
                if (validation != null)
                {
                    foreach (PatchValidationProps validationProp in validation.properties)
                    {
                        Match match = Regex.Match(patch.Field.ToLower(), validationProp.FieldRegex);

                        if (match.Success)
                        {
                            foundMatch = true;
                            var parameters = new
                            {
                                Child = match.Groups["child"].Value,
                                ChildId = match.Groups["childId"].Value,
                                Parent = validationProp.Parent,
                                ParentId = match.Groups["parentId"].Value
                            };
                            
                            validationProp.Validate(resource, patch, parameters);
                        }
                    }
                }
                else
                {
                    // _errors.Add(new Error)
                }

                if (!foundMatch)
                {
                    _errors.Add(new Error(1500, new { field = patch.Field.ToLower() }));
                }
            }
        }
    }
}
