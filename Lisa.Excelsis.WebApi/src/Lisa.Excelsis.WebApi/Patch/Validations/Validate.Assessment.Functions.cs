using System;
using System.Text.RegularExpressions;

namespace Lisa.Excelsis.WebApi
{
    partial class Validate
    {
        public bool ValidateAssessed(object resource, Patch patch, PatchPropInfo parameters)
        {
            DateTime temp;
            bool ValueIsValid = (DateTime.TryParse(patch.Value.ToString(), out temp)); 
            return (ValueIsValid);
        }

        public bool ValidateMark(object resource, Patch patch, PatchPropInfo parameters)
        {
            bool resourceExists = CheckResource(resource, "observations", parameters.ChildId);
            bool ValueIsValid = Regex.IsMatch(patch.Value.ToString(), parameters.Regex);
            return (resourceExists && ValueIsValid);
        }

        public bool ValidateResult(object resource, Patch patch, PatchPropInfo parameters)
        {
            bool resourceExists = CheckResource(resource, parameters.Child, parameters.ChildId);
            bool ValueIsValid = Regex.IsMatch(patch.Value.ToString(), parameters.Regex);
            return (resourceExists && ValueIsValid);
        }
    }
}
