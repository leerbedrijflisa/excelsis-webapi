using System.Collections.Generic;

namespace Lisa.Excelsis.WebApi
{
    partial class Database
    {
        public object FetchExam(string subject, string name, string cohort)
        {
            var query = FetchExamQuery +
                        @" WHERE Exams.Name = @Name
                             AND Exams.Subject = @Subject
                             AND Exams.Cohort = @Cohort
                           ORDER BY Categories.[Order] ASC, Criteria.[Order] ASC";

            var parameters = new {
                Subject = subject,
                Name = name,
                Cohort = cohort
            };

            return _gateway.SelectSingle(query, parameters);
        }

        public object FetchExam(object id)
        {
            var query = FetchExamQuery +
                        @" WHERE Exams.Id = @Id
                            ORDER BY Categories.[Order] ASC, Criteria.[Order] ASC";

            var parameters = new {
                Id = id
            };

            return _gateway.SelectSingle(query, parameters);
        }

        public IEnumerable<object> FetchExams(Filter filter)
        {
            var query = FetchExamsQuery +
                        @" ORDER BY Assessed DESC , Subject, Cohort desc, Exams.Name";

            var parameters = new
            {
                Assessor = filter.Assessor ?? string.Empty
            };

            return _gateway.SelectMany(query, parameters);
        }

        public IEnumerable<object> FetchExams(Filter filter, string subject, string cohort)
        {
            var query = FetchExamsQuery +
                        @" WHERE Subject = @Subject 
                             AND Cohort = @Cohort
                           ORDER BY Assessed DESC , Subject, Cohort desc, Exams.Name";

            var parameters = new {
                Subject = subject,
                Cohort = cohort,
                Assessor = filter.Assessor ?? string.Empty
            };

            return _gateway.SelectMany(query, parameters);
        }

        public object AddExam(ExamPost exam)
        {
            var query = @"INSERT INTO Exams (Name, Cohort, Crebo, Subject)
                          VALUES (@Name, @Cohort, @Crebo, @subject);";
            return _gateway.Insert(query, exam);
        }

        public bool ExamExists(ExamPost exam)
        {
            var query = @"SELECT COUNT(*) as count FROM Exams
                          WHERE Name LIKE @Name
                            AND Subject LIKE @Subject
                            AND Cohort LIKE @Cohort
                            AND Crebo LIKE @Crebo";
            dynamic result = _gateway.SelectSingle(query, exam);
            return (result.count > 0);
        }

        private string FetchExamQuery
        {
            get
            {
                return @"SELECT Exams.Id AS [@], Exams.Id, Exams.Name, Cohort, Crebo, Subject,
                                Categories.Id as #Categories_@Id,
                                Categories.Id as #Categories_Id,
                                Categories.[Order] as #Categories_Order,
                                Categories.Name as #Categories_Name,
                                Criteria.Id as #Categories_#Criteria_@Id,
                                Criteria.Id as #Categories_#Criteria_Id,
                                Criteria.[Order] as #Categories_#Criteria_Order,
                                Criteria.Title as #Categories_#Criteria_Title,
                                Criteria.[Description] as #Categories_#Criteria_Description,
                                Criteria.Value as #Categories_#Criteria_Value
                          FROM Exams
                LEFT JOIN Categories ON Categories.ExamId = Exams.Id
                LEFT JOIN Criteria ON Criteria.CategoryId = Categories.Id";
            }
        }

        private string FetchExamsQuery
        {
            get
            {
                return @"SELECT Id, Name, Cohort, Crebo, Subject
                          FROM Exams
                          LEFT JOIN (	
	                          SELECT TOP 10 Exam_Id, MAX(Assessments.Assessed) as Assessed
	                          FROM Assessments	
	                          LEFT JOIN AssessmentsAssessors ON AssessmentsAssessors.Assessment_Id = Assessments.Id
	                          LEFT JOIN Assessors ON Assessors.Id = AssessmentsAssessors.Assessor_Id
	                          WHERE Assessments.Assessed > DATEADD(Year,-1,GETDATE())
	                          AND Assessors.UserName = @Assessor
	                          GROUP BY Exam_Id
                          ) Assessments ON Exams.Id = Assessments.Exam_Id";
            }
        }
    }
}