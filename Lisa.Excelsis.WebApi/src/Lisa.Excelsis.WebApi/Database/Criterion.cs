using System.Collections.Generic;

namespace Lisa.Excelsis.WebApi
{
    partial class Database
    {
        public object AddCriterion(int id, CriterionPost criterion)
        {
            _errors = new List<Error>();

            dynamic exam = FetchExam(id);

            if (exam == null)
            {
                _errors.Add(new Error(1103, string.Format("The exam with id '{0}' was not found.", id), new
                {
                   Id = id
                }));
            }

            dynamic category = FetchCategory(criterion.CategoryId, id);

            if (category == null)
            {
                _errors.Add(new Error(1103, string.Format("The category with id '{0}' was not found by exam id {1}.",criterion.CategoryId, id), new
                {
                    CategoryId = criterion.CategoryId,
                    ExamId = id
                }));
            }

            if (_errors.Count == 0)
            {
                var query = @"INSERT INTO Criteria ([Order], Title, [Description], Value, ExamId, CategoryId)
                          VALUES (@Order, @Title ,@Description, @Value, @ExamId, @CategoryId);";

                var parameters = new
                {
                    Order = criterion.Order,
                    Title = criterion.Title,
                    Description = criterion.Description,
                    Value = criterion.Value,
                    CategoryId = criterion.CategoryId,
                    ExamId = id
                };

                return _gateway.Insert(query, parameters);
            }
            return null;
        }
    }
}