using System.Collections.Generic;

namespace Lisa.Excelsis.WebApi
{
    partial class Database
    {
        public object FetchExam(int id)
        {
            var query = @"SELECT * FROM Exam WHERE Exam.Id = @id";
            var parameters = new { id = id };
            return _gateway.SelectSingle(query, parameters);
        }

        public IEnumerable<object> FetchExams()
        {
            var query = @"SELECT * FROM Exam";
            return _gateway.SelectMany(query);
        }

        public object AddExam(ExamPost exam)
        {
            var query = @"INSERT INTO Exam (Name, Cohort, Crebo, Subject)
                          VALUES (@Name, @Cohort, @Crebo, @subject);";
            var parameters = new { Name = exam.Name, Cohort = exam.Cohort, Crebo = exam.Crebo, Subject = exam.Subject };
            return _gateway.Insert(query, parameters);
        }           
    }
}
