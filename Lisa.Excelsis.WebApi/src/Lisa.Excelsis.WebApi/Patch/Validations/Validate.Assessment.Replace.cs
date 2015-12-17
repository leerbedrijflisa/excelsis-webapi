using System.Text.RegularExpressions;

namespace Lisa.Excelsis.WebApi
{
    partial class Validate
    {
        public bool ValidateStudentName(object resource, Patch patch, dynamic parameters)
        {
            bool resourceExists = CheckResource(resource, parameters.Child, parameters.ChildId);
            bool ValueIsValid = Regex.IsMatch(patch.Value.ToString(), @"^\d+$");
            return (resourceExists && ValueIsValid);
        }

        public bool ValidateStudentNumber(object resource, Patch patch, dynamic parameters)
        {
            bool resourceExists = CheckResource(resource, parameters.Child, parameters.ChildId);
            bool ValueIsValid = Regex.IsMatch(patch.Value.ToString(), @"^\d+$");
            return (resourceExists && ValueIsValid);
        }

        public bool ValidateAssessed(object resource, Patch patch, dynamic parameters)
        {
            bool resourceExists = CheckResourceInResource(resource, parameters.Parent, parameters.ParentId, parameters.Child, parameters.ChildId);
            bool ValueIsValid = Regex.IsMatch(patch.Value.ToString(), @"^\d+$");
            return (resourceExists && ValueIsValid);
        }

        public bool ValidateObservationResult(object resource, Patch patch, dynamic parameters)
        {
            bool resourceExists = CheckResourceInResource(resource, parameters.Parent, parameters.ParentId, parameters.Child, parameters.ChildId);
            bool ValueIsValid = Regex.IsMatch(patch.Value.ToString(), @"^\d+$");
            return (resourceExists && ValueIsValid);
        }
    }
}
