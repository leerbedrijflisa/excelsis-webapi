using System.Collections.Generic;

namespace Lisa.Excelsis.WebApi
{
    partial class Database
    {
        public object FetchCategory(int id, int examId)
        {
            var query = @"SELECT Id, [Order], Name FROM Categories 
                          WHERE Categories.Id = @Id AND Categories.ExamId = @ExamId";
            return _gateway.SelectSingle(query, new { Id = id, ExamId = examId });
        }

        public void AddCategories(dynamic assessmentResult, dynamic examResult)
        {
            if (examResult.Categories.Count > 0)
            {
                foreach (var category in examResult.Categories)
                {
                    var query = @"INSERT INTO AssessmentCategories ([Order], Name, AssessmentId) 
                                  VALUES (@Order, @Name, @Assessment)";
                    _gateway.Insert(query, new { Order = category.Order, Name = category.Name, Assessment = assessmentResult });
                }
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