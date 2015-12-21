using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Lisa.Excelsis.WebApi
{
    partial class Validate
    {
        public bool CheckResource(dynamic resource, string child, object childId)
        {
            var query = @"SELECT COUNT(*) as count FROM " + child + @"
                          WHERE " + resource.Name + @" = " + resource.Value + @" AND Id = @Id";
            var parameters = new { id = childId };

            dynamic result = _db.Execute(query, parameters);
            return (result.count > 0);
        }

        public bool CheckResourceInResource(dynamic resource, string parent, string parentId, string child, object childId)
        {
            var query = @"SELECT COUNT(*) as count FROM " + child + @"
                          WHERE " + resource.Name + @" = " + resource.Value + @" AND Id = @Id AND " + parent + @" = @ParentId";
            var parameters = new { Id = childId, ParentId = parentId };

            dynamic result = _db.Execute(query, parameters);
            return (result.count > 0);
        }

        public bool CheckValue(Patch patch, Dictionary<string, string> dict)
        {
            int Count = 0;
            string regex;
            var value = patch.Value as JObject;
            if (value != null)
            {
                foreach(var prop in value)
                {
                    if (dict.TryGetValue(prop.Key, out regex))
                    {
                        if (Regex.IsMatch(prop.Value.ToString(), regex))
                        {
                            Count++;
                        }
                        else
                        {
                            // TODO: Error value is not correct
                        }
                    }
                    else
                    {
                        // TODO: Error property is not correct
                    }
                }
            }
            else
            {
                // TODO: Error value cannot be parsed to object
            }
            return (Count == dict.Count);
        }

        private static readonly Database _db = new Database();
    }
}
