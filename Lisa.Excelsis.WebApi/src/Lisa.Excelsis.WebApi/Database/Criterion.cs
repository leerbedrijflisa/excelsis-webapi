using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Lisa.Excelsis.WebApi
{
    partial class Database
    {
        public void AddCriterion(int id, int categoryId, Patch patch)
        {
            _errors = new List<Error>();
            Dictionary<string, string> dict = new Dictionary<string, string>();

            foreach (var propPatch in (JObject)patch.Value)
            {
                if (Regex.IsMatch(propPatch.Key.ToLower(), @"^order$")
                    || Regex.IsMatch(propPatch.Key.ToLower(), @"^weight$")
                    || Regex.IsMatch(propPatch.Key.ToLower(), @"^title$")
                    || Regex.IsMatch(propPatch.Key.ToLower(), @"^description$"))
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

            if (!dict.ContainsKey("title"))
            {
                _errors.Add(new Error(1101, new { field = "title" }));
            }

            if (!dict.ContainsKey("description"))
            {
                _errors.Add(new Error(1101, new { field = "description" }));
            }

            if (!dict.ContainsKey("weight"))
            {
                _errors.Add(new Error(1101, new { field = "weight" }));
            }

            if(_errors.Count > 0)
            {
                return;
            }

            if (!Regex.IsMatch(dict["order"].ToString(), @"^\d+$"))
            {
                _errors.Add(new Error(1202, new { field = "order", value = dict["order"].ToString() }));
            }

            if (!Regex.IsMatch(dict["weight"].ToString(), @"^(fail|pass|excellent)$"))
            {
                _errors.Add(new Error(1204, new { field = "weight", value = dict["weight"].ToString(), permitted = new string[] { "fail", "pass", "excellent" } }));
            }

            if (_errors.Count == 0)
            {
                var query = @"INSERT INTO Criteria ([Order], Title, [Description], weight, ExamId, CategoryId)
                        VALUES (@Order, @Title ,@Description, @Weight, @ExamId, @CategoryId);";

                var parameters = new
                {
                    Order = dict["order"],
                    Title = dict["title"],
                    Description = dict["description"],
                    Weight = dict["weight"],
                    CategoryId = categoryId,
                    ExamId = id
                };

                _gateway.Insert(query, parameters);
            }
        }
    }
}