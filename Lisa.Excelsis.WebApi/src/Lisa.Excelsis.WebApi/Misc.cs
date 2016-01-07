using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Lisa.Excelsis.WebApi
{
    public static class Misc
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
    }
}
