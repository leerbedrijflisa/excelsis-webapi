using System.Collections.Generic;
using System.Reflection;

namespace Lisa.Excelsis.WebApi
{
    internal class SubObjectRowProvider : IRowProvider
    {
        public SubObjectRowProvider(string prefix, IRowProvider data)
        {
            _prefix = prefix + "_";
            _data = data;
        }

        public IEnumerable<KeyValuePair<string, object>> Fields
        {
            get
            {
                foreach (var field in _data.Fields)
                {
                    if (field.Key.StartsWith(_prefix))
                    {
                        var name = field.Key.Substring(_prefix.Length);
                        yield return new KeyValuePair<string, object>(name, field.Value);
                    }
                }
            }
        }

        private IRowProvider _data;
        private string _prefix;
    }
}