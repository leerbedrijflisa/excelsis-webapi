using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Lisa.Excelsis.WebApi
{
    public static class Utils
    {
        public static string CleanParam(string name)
        {
            List<string> nameParts = new List<string>();
            Regex regex = new Regex(@"[\w\d\.]+");
            var matches = regex.Matches(name.ToLower());
            foreach (Match match in matches)
            {
                nameParts.Add(match.Value);
            }
            return string.Join("-", nameParts);
        }

        public static bool TryValidate(object @object, out List<ValidationResult> results)
        {
            var context = new ValidationContext(@object, serviceProvider: null, items: null);
            results = new List<ValidationResult>();
            return System.ComponentModel.DataAnnotations.Validator.TryValidateObject(
                @object, context, results,
                validateAllProperties: true
            );
        }
    }
}
