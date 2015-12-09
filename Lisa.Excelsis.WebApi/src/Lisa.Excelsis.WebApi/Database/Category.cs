using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Lisa.Excelsis.WebApi
{
    partial class Database
    {
        public object FetchCategory(int id, int examId)
        {
            var query = @"SELECT Id, [Order], Name
                          FROM Categories 
                          WHERE Categories.Id = @Id AND Categories.ExamId = @ExamId";

            var parameters = new
            {
                Id = id,
                ExamId = examId
            };

            return _gateway.SelectSingle(query, parameters);
        }

        public void AddCategory(int examId, Patch patch)
        {
            _errors = new List<Error>();
            Dictionary<string, string> dict = new Dictionary<string, string>();
            if (patch.Value != null)
            {

                foreach (var propPatch in (JObject)patch.Value)
                {
                    if (Regex.IsMatch(propPatch.Key.ToLower(), @"^order")
                     || Regex.IsMatch(propPatch.Key.ToLower(), @"^name"))
                    {
                        dict.Add(propPatch.Key.ToLower(), propPatch.Value.ToString());
                    }
                    else
                    {
                        _errors.Add(new Error(1205, new { field = propPatch.Key }));
                    }
                }

                if (!dict.ContainsKey("order"))
                {
                    _errors.Add(new Error(1101, new { field = "order" }));
                }

                if (!dict.ContainsKey("name"))
                {
                    _errors.Add(new Error(1101, new { field = "name" }));
                }

                if (_errors.Count > 0)
                {
                    return;
                }

                if (!Regex.IsMatch(dict["order"].ToString(), @"^\d+$"))
                {
                    _errors.Add(new Error(1202, new { field = "order", value = dict["order"].ToString() }));
                }

                if (_errors.Count == 0)
                {
                    var query = @"INSERT INTO Categories ([Order], Name, ExamId)
                            VALUES (@Order, @Name ,@ExamId);";
                    var parameters = new
                    {
                        Order = dict["order"],
                        Name = dict["name"],
                        ExamId = examId
                    };
                    _gateway.Insert(query, parameters);
                }
            }
            else
            {
                _errors.Add(new Error(1101, new { field = "value" }));
            }
        }

        public bool CategoryExists(int examId, int id)
        {
            _errors = new List<Error>();

            var query = @"SELECT COUNT(*) as count FROM Categories
                          WHERE ExamId = @ExamId AND Id = @Id";
            dynamic result = _gateway.SelectSingle(query, new { ExamId = examId, Id = id });

            return (result.count > 0);
        }
    }
}
