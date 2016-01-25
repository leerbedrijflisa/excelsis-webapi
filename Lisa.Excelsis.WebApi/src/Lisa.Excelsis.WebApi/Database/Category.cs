using System.Collections.Generic;

namespace Lisa.Excelsis.WebApi
{
    partial class Database
    {
        public object FetchCategory(int id, int examId)
        {
            var query = @"SELECT Id, [Order], Name
                          FROM Categories 
                          WHERE Categories.Id = @Id AND Categories.ExamId = @ExamId";
            return _gateway.SelectSingle(query, new { Id = id, ExamId = examId });
        }

        public void AddCategories(dynamic assessmentResult, dynamic examResult)
        {
            List<string> categories = new List<string>();
            if (examResult.Categories.Count > 0)
            {
                foreach (var category in examResult.Categories)
                {
                    categories.Add("(" + category.Order + ", '" + category.Name + "', " + assessmentResult + ")");
                }

                var query = @"INSERT INTO AssessmentCategories ([Order], Name, AssessmentId) VALUES ";
                query += string.Join(",", categories);
                _gateway.Insert(query, null);
            }
        }

        public bool CategoryExists(object examId, object id)
        {
            var query = @"SELECT COUNT(*) as count FROM Categories
                          WHERE ExamId = @ExamId AND Id = @Id";
            dynamic result = _gateway.SelectSingle(query, new { ExamId = examId, Id = id });

            return (result.count > 0);
        }
    }
}