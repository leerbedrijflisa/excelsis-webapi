using System.Collections.Generic;

namespace Lisa.Excelsis.WebApi
{
    partial class Database
    {
        public object FetchExam(string subject, string name, string cohort)
        {
            var query = FetchExamQuery + " WHERE Exams.Name = @Name AND Exams.Subject = @Subject AND Exams.Cohort = @Cohort";
            var parameters = new { Subject = subject, Name = name, Cohort = cohort};
            return _gateway.SelectSingle(query, parameters);
        }

        public object FetchExam(int id)
        {
            var query = FetchExamQuery + " WHERE Exams.Id = @Id";
            var parameters = new { Id = id };
            return _gateway.SelectSingle(query, parameters);
        }       

        public IEnumerable<object> FetchExams()
        {
            var query = @"SELECT Id, Name, Cohort, Crebo, Subject  FROM Exams";
            return _gateway.SelectMany(query);
        }

        public object AddExam(ExamPost exam)
        {
            var query = @"INSERT INTO Exams (Name, Cohort, Crebo, Subject)
                          VALUES (@Name, @Cohort, @Crebo, @subject);";
            var parameters = new { Name = exam.Name, Cohort = exam.Cohort, Crebo = exam.Crebo, Subject = exam.Subject };
            return _gateway.Insert(query, parameters);
        }           
        public bool AnyExam(ExamPost exam)
        {
            var query = @"SELECT COUNT(*) as count FROM Exams
                          WHERE Name LIKE @Name 
                            AND Subject LIKE @Subject 
                            AND Cohort LIKE @Cohort 
                            AND Crebo LIKE @Crebo";
            var parameters = new { Name = exam.Name, Subject = exam.Subject, Cohort = exam.Cohort, Crebo = exam.Crebo };
            dynamic result = _gateway.SelectSingle(query, parameters);

            return (result.count > 0) ? true : false;
        }

        private string FetchExamQuery
        {
            get
            {
                return @"SELECT Exams.Id AS [@], Exams.Id, Name, Cohort, Crebo, Subject, 
                                Criteriums.Id as #Criteriums_@ID, 
                                Criteriums.[Order] as #Criteriums_Order, 
                                Criteriums.[Description] as #Criteriums_Description, 
                                Criteriums.Value as #Criteriums_Value
                          FROM Exams
                          LEFT JOIN Criteriums ON Criteriums.ExamId = Exams.Id";
            }
        }
    }
}
