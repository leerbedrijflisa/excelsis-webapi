using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Lisa.Excelsis.WebApi
{
    partial class Database
    {
        public void AddCriterion(int id, int categoryId, Patch patch)
        {
            _errors = new List<Error>();
            Dictionary<string, string> dict = new Dictionary<string, string>();
            var fields = new List<string>() { "order", "weight", "title", "description" };
            var regex = @"^order$|^weight$|^title$|^description$";

            dict = IsPatchable(patch, fields, regex);

            FieldsExists(dict, fields);

            if (_errors.Any())
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

            if (!_errors.Any())
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