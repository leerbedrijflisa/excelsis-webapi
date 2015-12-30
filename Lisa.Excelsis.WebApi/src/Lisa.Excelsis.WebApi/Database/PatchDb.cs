using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Lisa.Excelsis.WebApi
{
    partial class Database
    {
        public void Patch(dynamic resource, IEnumerable<Patch> patches, IEnumerable<PatchValidation> validations)
        {
            _errors = new List<Error>();
            foreach (Patch patch in patches)
            {
                if (patch.Field == null)
                {
                    _errors.Add(new Error(1101, new ErrorProps { Field = "field" }));
                }
                if (patch.Action == null)
                {
                    _errors.Add(new Error(1101, new ErrorProps { Field = "action" }));
                }
                if (patch.Value == null)
                {
                    _errors.Add(new Error(1101, new ErrorProps { Field = "value" }));
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
                                var parameters = new PatchPropInfo
                                {
                                    Child = (match.Groups["child"].Value == string.Empty)? validationProp.Parent : match.Groups["child"].Value,
                                    ChildId = (match.Groups["childId"].Value == string.Empty)? resource.Value.ToString() : match.Groups["childId"].Value,
                                    Parent = validationProp.Parent,
                                    ParentId = match.Groups["parentId"].Value,
                                    Property = match.Groups["property"].Value,
                                    Target = match.Groups["target"].Value,
                                    Regex = validationProp.ValueRegex,
                                    ErrorInfo = validationProp.ErrorInfo
                                };

                                if(parameters.ErrorInfo == null)
                                {
                                    parameters.ErrorInfo = new ErrorProps();
                                }

                                parameters.ErrorInfo.Value = patch.Value.ToString();

                                var ValidResource = validationProp.Validate(resource, patch, parameters);
                                if (ValidResource)
                                {
                                    validationProp.BuildQuery(resource, patch.Value, parameters);
                                }
                            }
                        }

                        if (!foundMatch)
                        {
                            _errors.Add(new Error(1500, new ErrorProps { Field = "field" }));
                        }
                    }
                    else
                    {
                        _errors.Add(new Error(1208, new ErrorProps { Field = "value", Type = "object" }));
                    }
                }
            }
        }
    }
}
