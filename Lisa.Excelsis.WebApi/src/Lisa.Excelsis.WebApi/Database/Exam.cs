using System;
using System.Collections.Generic;

namespace Lisa.Excelsis.WebApi
{
    partial class Database
    {
        public object FetchExam(int id)
        {
            var query = @"SELECT Exam.Id AS [@], Exam.*, Criterium.Id as #Criterium_@ID, Criterium.[Order] as #Criterium_Order, Criterium.[Description] as #Criterium_Description, Criterium.Value as #Criterium_Value
                          FROM Exam
                          LEFT JOIN Criterium ON Criterium.ExamId = @id
                          WHERE Exam.Id = @id";
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
    }
}
