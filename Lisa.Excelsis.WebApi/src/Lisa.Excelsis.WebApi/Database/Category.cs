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
    }
}
