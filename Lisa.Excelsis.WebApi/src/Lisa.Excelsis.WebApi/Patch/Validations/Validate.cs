using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Lisa.Excelsis.WebApi
{
    partial class Validate
    {
        public IEnumerable<Error> Errors
        {
            get
            {
                return _errors;
            }
        }

        public void ClearErrors()
        {
            _errors.Clear();
        }

        public static List<Error> _errors { get; set; }

        private static readonly Database _db = new Database();
    }
}
