using System.Collections.Generic;
using System.Linq;
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
            var fields = new List<string>() { "order", "name" };
            var regex = @"^(order|name)$";

            dict = IsPatchable(patch,fields, regex);

            FieldsExists(dict, fields);

            if (_errors.Any())
            {
                return;
            }

            if (!Regex.IsMatch(dict["order"].ToString(), @"^\d+$"))
            {
                _errors.Add(new Error(1202, new { field = "order", value = dict["order"].ToString() }));
            }

            if (!_errors.Any())
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

        public bool CategoryExists(int examId, int id)
        {
            var query = @"SELECT COUNT(*) as count FROM Categories
                          WHERE ExamId = @ExamId AND Id = @Id";
            dynamic result = _gateway.SelectSingle(query, new { ExamId = examId, Id = id });

            return (result.count > 0);
        }
    }
}
