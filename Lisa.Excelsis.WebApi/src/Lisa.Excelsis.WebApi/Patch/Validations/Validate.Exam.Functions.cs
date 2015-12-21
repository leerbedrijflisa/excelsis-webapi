using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Lisa.Excelsis.WebApi
{
    partial class Validate
    {
        /*
         *  Validate Add actions
         */
        public bool ValidateCategory(object resource, Patch patch, PatchPropInfo parameters)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>()
            {
                { "order", @"^\d+$" },
                { "name", @"^[a-zA-Z\s,!?.:'""]*$" }
            };

            return CheckValue(patch, dict);
        }

        public bool ValidateCriterion(object resource, Patch patch, PatchPropInfo parameters)
        {
            if (CheckResource(resource, parameters.Child, parameters.ChildId))
            {
                Dictionary<string, string> dict = new Dictionary<string, string>()
                {
                    { "order", @"^\d+$" },
                    { "title", @"^[a-zA-Z\s,!?.:'""]*$" },
                    { "description", @"^[a-zA-Z\s,!?.:'""]*$" },
                    { "weight", @"^(fail|pass|excellent)$"}
                };

                return CheckValue(patch, dict);
            }
            else
            {
                // TODO: Error category does not exist
            }
            return false;
        }
        
        /*
         *  Validate Replace actions
         */

        public bool ValidateReplaceCategory(object resource, Patch patch, PatchPropInfo parameters)
        {
            bool resourceExists = CheckResource(resource, parameters.Child, parameters.ChildId);
            bool ValueIsValid = Regex.IsMatch(patch.Value.ToString(), parameters.Regex);
            return (resourceExists && ValueIsValid);
        }

        public bool ValidateReplaceCriterion(object resource, Patch patch, PatchPropInfo parameters)
        {
            bool resourceExists = CheckResourceInResource(resource, parameters.Parent, parameters.ParentId, parameters.Child, parameters.ChildId);
            bool ValueIsValid = Regex.IsMatch(patch.Value.ToString(), parameters.Regex);
            return (resourceExists && ValueIsValid);
        }
    }
}
