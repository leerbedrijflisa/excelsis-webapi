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
            CriterionPost criterion = new CriterionPost();
            Dictionary<string, string> dict = new Dictionary<string, string>();

            foreach (var propPatch in (JObject)patch.Value)
            {
                if ((propPatch.Key.ToLower() == "order" && Regex.IsMatch(propPatch.Value.ToString(), @"^\d+$"))
                 || (propPatch.Key.ToLower() == "value" && Regex.IsMatch(propPatch.Value.ToString(), @"^fail$|^pass$|^excellent$"))
                 || (propPatch.Key.ToLower() == "title")
                 || (propPatch.Key.ToLower() == "description"))
                {
                    dict.Add(propPatch.Key.ToLower(), propPatch.Value.ToString());
                }
                else
                {
                    _errors.Add(new Error(0, string.Format("The field '{0}' with value '{1}' is not patchable", propPatch.Key, propPatch.Value.ToString()), new
                    {
                        Key = propPatch.Key,
                        Value = propPatch.Value.ToString()
                    }));
                }
            }

            if (!dict.ContainsKey("order"))
            {
                _errors.Add(new Error(1111, "The field 'Order' is required.", new { field = "Order" }));
            }

            if (!dict.ContainsKey("title"))
            {
                _errors.Add(new Error(1111, "The field 'Title' is required.", new { field = "Title" }));
            }

            if (!dict.ContainsKey("description"))
            {
                _errors.Add(new Error(1111, "The field 'Description' is required.", new { field = "Description" }));
            }

            if (!dict.ContainsKey("value"))
            {
                _errors.Add(new Error(1111, "The field 'Value' is required.", new { field = "Value" }));
            }


            if (_errors.Count == 0)
            {
               
                var query = @"INSERT INTO Criteria ([Order], Title, [Description], Value, ExamId, CategoryId)
                            VALUES (@Order, @Title ,@Description, @Value, @ExamId, @CategoryId);";
                
                var parameters = new
                {
                    Order = dict["order"],
                    Title = dict["title"],
                    Description = dict["description"],
                    Value = dict["value"],
                    CategoryId = categoryId,
                    ExamId = id
                };

                _gateway.Insert(query, parameters);
            }
        }
    }
}