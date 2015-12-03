using Lisa.Common.Sql;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Lisa.Excelsis.WebApi
{
    partial class Database : IDisposable
    {
        public void Dispose()
        {
            _gateway?.Dispose();
        }

        public IEnumerable<Error> Errors
        {
            get
            {
                return _errors;
            }
        }

        public string CleanParam(string name)
        {
            List<string> nameParts = new List<string>();
            Regex regex = new Regex(@"[\w\d\.]+");
            var matches = regex.Matches(name.ToLower());
            foreach(Match match in matches)
            {
                nameParts.Add(match.Value);
            }
            return string.Join("-", nameParts);
        }

        private List<Error> _errors { get; set; }

        private Gateway _gateway = new Gateway(Environment.GetEnvironmentVariable("ConnectionString"));
    }
}