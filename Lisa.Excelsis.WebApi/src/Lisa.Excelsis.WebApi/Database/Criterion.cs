namespace Lisa.Excelsis.WebApi
{
    partial class Database
    {
        public bool CriterionExists(int examId, object cid, object id)
        {
            var query = @"SELECT COUNT(*) as count FROM Criteria
                          WHERE ExamId = @ExamId AND CategoryId = @Cid AND Id = @Id";
            dynamic result = _gateway.SelectSingle(query, new { ExamId = examId, Cid = cid, Id = id });

            return (result.count > 0);
        }
    }
}