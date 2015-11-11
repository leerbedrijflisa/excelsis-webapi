namespace Lisa.Excelsis.WebApi
{
    partial class Database
    {
        public object AddCriterium(int id, CriteriumPost criterium)
        {
            var query = @"INSERT INTO Criterium ([Order], [Description], [Value], [ExamId])
                          VALUES (@Order, @Description, @Value, @ExamId);";
            var parameters = new { Order = criterium.Order, Description = criterium.Description, Value = criterium.Value, ExamId = id };
            return _gateway.Insert(query, parameters);
        }
        public bool AnyCriterium(int id, CriteriumPost criterium)
        {
            var query = @"SELECT COUNT(*) as count FROM Criterium 
                          WHERE [Order] LIKE @Order 
                            AND ExamId LIKE @ExamId";
            var parameters = new { Order = criterium.Order, Description = criterium.Description, Value = criterium.Value, ExamId = id };
            dynamic result = _gateway.SelectSingle(query, parameters);

            return (result.count > 0) ? true : false;
        }
    }
}
