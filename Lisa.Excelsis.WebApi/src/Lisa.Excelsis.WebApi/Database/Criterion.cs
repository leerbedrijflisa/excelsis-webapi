namespace Lisa.Excelsis.WebApi
{
    partial class Database
    {
        public object AddCriterion(int id, CriterionPost criterion)
        {
            var query = @"INSERT INTO Criteria ([Order], Title, [Description], Value, ExamId)
                          VALUES (@Order, @Title ,@Description, @Value, @ExamId);";

            var parameters = new {
                Order = criterion.Order,
                Title = criterion.Title,
                Description = criterion.Description,
                Value = criterion.Value,
                ExamId = id
            };

            return _gateway.Insert(query, parameters);
        }

        public bool CriterionExists(int id, CriterionPost criterion)
        {
            var exam = FetchExam(id);
            if (exam != null)
            {
                var query = @"SELECT COUNT(*) as count FROM Criteria
                          WHERE [Order] = @Order
                            AND ExamId = @ExamId";

                var parameters = new
                {
                    Order = criterion.Order,
                    Description = criterion.Description,
                    Value = criterion.Value,
                    ExamId = id
                };

                dynamic result = _gateway.SelectSingle(query, parameters);
                return (result.count > 0);
            }
            return true;
        }
    }
}