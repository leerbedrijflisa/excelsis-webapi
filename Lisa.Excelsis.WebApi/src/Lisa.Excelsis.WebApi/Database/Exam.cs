using System.Collections.Generic;

namespace Lisa.Excelsis.WebApi
{
    partial class Database
    {
        public object FetchExam(string subject, string name, string cohort)
        {
            var query = FetchExamQuery + " WHERE Exam.Name = @Name AND Exam.Subject = @Subject AND Exam.Cohort = @Cohort";
            var parameters = new { Subject = subject, Name = name, Cohort = cohort};
            return _gateway.SelectSingle(query, parameters);
        }

        public object FetchExam(int id)
        {
            var query = FetchExamQuery + " WHERE Exam.Id = @Id";
            var parameters = new { Id = id };
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
        public bool AnyExam(ExamPost exam)
        {
            var query = @"SELECT COUNT(*) as count FROM Exam 
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
                return @"SELECT Exam.Id AS [@], Name, Cohort, Crebo, Subject, 
                                Criterium.Id as #Criterium_@ID, 
                                Criterium.[Order] as #Criterium_Order, 
                                Criterium.[Description] as #Criterium_Description, 
                                Criterium.Value as #Criterium_Value
                          FROM Exam
                          LEFT JOIN Criterium ON Criterium.ExamId = Exam.Id";
            }
        }
    }
}
