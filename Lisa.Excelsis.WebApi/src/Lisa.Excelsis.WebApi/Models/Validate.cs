using System.Text.RegularExpressions;

namespace Lisa.Excelsis.WebApi
{
    partial class Validate
    {
        public bool ValidateCategoryOrder(object resource, Patch patch, dynamic parameters)
        {
            bool resourceExists = _db.CheckResource(resource, parameters.Child, parameters.ChildId);
            bool ValueIsValid = Regex.IsMatch(patch.Value.ToString(), @"^\d+$");
            return (resourceExists && ValueIsValid);
        }

        public bool ValidateCategoryName(object resource, Patch patch, dynamic parameters)
        {
            bool resourceExists = _db.CheckResource(resource, parameters.Child, parameters.ChildId);
            bool ValueIsValid = Regex.IsMatch(patch.Value.ToString(), @"^\d+$");
            return (resourceExists && ValueIsValid);
        }

        public bool ValidateCriterionOrder(object resource, Patch patch, dynamic parameters)
        {
            bool resourceExists = _db.CheckResourceInResource(resource, parameters.Parent, parameters.ParentId, parameters.Child, parameters.ChildId);
            bool ValueIsValid = Regex.IsMatch(patch.Value.ToString(), @"^\d+$");
            return (resourceExists && ValueIsValid);
        }

        public bool ValidateCriterionTitle(object resource, Patch patch, dynamic parameters)
        {
            bool resourceExists = _db.CheckResourceInResource(resource, parameters.Parent, parameters.ParentId, parameters.Child, parameters.ChildId);
            bool ValueIsValid = Regex.IsMatch(patch.Value.ToString(), @"^\d+$");
            return (resourceExists && ValueIsValid);
        }

        public bool ValidateCriterionDescription(object resource, Patch patch, dynamic parameters)
        {
            bool resourceExists = _db.CheckResourceInResource(resource, parameters.Parent, parameters.ParentId, parameters.Child, parameters.ChildId);
            bool ValueIsValid = Regex.IsMatch(patch.Value.ToString(), @"^\d+$");
            return (resourceExists && ValueIsValid);
        }

        public bool ValidateCriterionWeight(object resource, Patch patch, dynamic parameters)
        {
            bool resourceExists = _db.CheckResourceInResource(resource, parameters.Parent, parameters.ParentId, parameters.Child, parameters.ChildId);
            bool ValueIsValid = Regex.IsMatch(patch.Value.ToString(), @"^\d+$");
            return (resourceExists && ValueIsValid);
        }

        private readonly Database _db = new Database();
    }
}
