using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Lisa.Excelsis.WebApi
{
    partial class Database
    {
        public object FetchExam(string subject, string name, string cohort)
        {
            var query = FetchExamQuery +
                        @" WHERE Exams.NameId = @Name
                             AND Exams.SubjectId = @Subject
                             AND Exams.Cohort = @Cohort
                           ORDER BY Categories.[Order] ASC, Criteria.[Order] ASC";
            return _gateway.SelectSingle(query, new { Subject = subject, Name = name, Cohort = cohort });
        }

        public object FetchExam(object id)
        {
            var query = FetchExamQuery +
                        @" WHERE Exams.Id = @Id
                            ORDER BY Categories.[Order] ASC, Criteria.[Order] ASC";
            return _gateway.SelectSingle(query, new { Id = id });
        }

        public IEnumerable<object> FetchExams(Filter filter)
        {
            var query = FetchExamsQuery +
                        @" ORDER BY Assessed DESC , Subject, Cohort desc, Exams.Name";
            return _gateway.SelectMany(query, new { Assessor = filter.Assessors ?? string.Empty });
        }

        public IEnumerable<object> FetchExams(Filter filter, string subject, string cohort)
        {
            var query = FetchExamsQuery +
                        @" WHERE SubjectId = @Subject 
                             AND Cohort = @Cohort
                           ORDER BY Assessed DESC , Subject, Cohort desc, Exams.Name";

            var parameters = new {
                Subject = subject,
                Cohort = cohort,
                Assessor = filter.Assessors ?? string.Empty
            };

            return _gateway.SelectMany(query, parameters);
        }

        public object AddExam(ExamPost exam)
        {
            _errors = new List<Error>();
            string subjectId = Misc.CleanParam(exam.Subject);
            string nameId = Misc.CleanParam(exam.Name);
            exam.Crebo = (exam.Crebo == null) ? string.Empty : exam.Crebo;

            if (!Regex.IsMatch(exam.Crebo, @"^$|^\d{5}$"))
            {
                _errors.Add(new Error(1203, new ErrorProps { Field = "crebo", Value = exam.Crebo, Count = 5 }));
            }

            if (!Regex.IsMatch(exam.Cohort, @"^(19|20)\d{2}$"))
            {
                _errors.Add(new Error(1207, new ErrorProps { Field = "cohort", Value = exam.Cohort, Count = 4 , Min = 1900, Max = 2099 }));
            }

            if (subjectId == string.Empty)
            {
                _errors.Add(new Error(1206, new ErrorProps { Field = "Subject", Value = exam.Subject }));
            }

            if (nameId == string.Empty)
            {
                _errors.Add(new Error(1206, new ErrorProps { Field = "Name", Value = exam.Name }));
            }

            if(!Regex.IsMatch(exam.Status, @"^(draft|published)$"))
            {
                _errors.Add(new Error(1204, new ErrorProps { Field = "status", Value = exam.Status, Permitted1 = "draft", Permitted2 = "published" }));
            }

            if (_errors.Any())
            {
                return null;
            }

            var query = @"INSERT INTO Exams (Name, NameId, Cohort, Crebo, Subject, SubjectId, Status)
                        VALUES (@Name, @NameId, @Cohort, @Crebo, @subject, @SubjectId, @Status);";
            var parameters = new
            {
                Name = exam.Name,
                NameId = nameId,
                Cohort = exam.Cohort,
                Crebo = exam.Crebo,
                Subject = exam.Subject,
                SubjectId = subjectId,
                Status = exam.Status
            };
            return _gateway.Insert(query, parameters);
        }

        public void PatchExam(IEnumerable<Patch> patches, int id)
        {
            ExamBuilder builder = new ExamBuilder();
            builder.BuildPatches(id, patches);
        }
        
        public bool ExamExists(ExamPost exam)
        {
            var query = @"SELECT COUNT(*) as count FROM Exams
                          WHERE NameId = @Name
                            AND SubjectId = @Subject
                            AND Cohort = @Cohort
                            AND Crebo = @Crebo";

            var parameters = new
            {
                Name = Misc.CleanParam(exam.Name),
                Subject = Misc.CleanParam(exam.Subject),
                Cohort = exam.Cohort,
                Crebo = exam.Crebo ?? string.Empty
            };
            dynamic result = _gateway.SelectSingle(query, parameters);

            return (result.count > 0);
        }

        public bool ExamExists(int id)
        {
            var query = @"SELECT COUNT(*) as count FROM Exams
                          WHERE Id = @Id";
            dynamic result = _gateway.SelectSingle(query, new { Id = id });

            return (result.count > 0);
        }

        private string FetchExamQuery
        {
            get
            {
                return @"SELECT Exams.Id AS [@], Exams.Id, Exams.Name, Cohort, Crebo, Subject, Status,
                                Categories.Id as #Categories_@Id,
                                Categories.Id as #Categories_Id,
                                Categories.[Order] as #Categories_Order,
                                Categories.Name as #Categories_Name,
                                Criteria.Id as #Categories_#Criteria_@Id,
                                Criteria.Id as #Categories_#Criteria_Id,
                                Criteria.[Order] as #Categories_#Criteria_Order,
                                Criteria.Title as #Categories_#Criteria_Title,
                                Criteria.[Description] as #Categories_#Criteria_Description,
                                Criteria.Weight as #Categories_#Criteria_Weight
                          FROM Exams
                LEFT JOIN Categories ON Categories.ExamId = Exams.Id
                LEFT JOIN Criteria ON Criteria.CategoryId = Categories.Id";
            }
        }

        private string FetchExamsQuery
        {
            get
            {
                return @"SELECT Id, Name, Cohort, Crebo, Subject, Status
                          FROM Exams
                          LEFT JOIN (	
	                          SELECT TOP 10 ExamId, MAX(Assessments.Assessed) as Assessed
	                          FROM Assessments	
	                          LEFT JOIN AssessmentsAssessors ON AssessmentsAssessors.AssessmentId = Assessments.Id
	                          LEFT JOIN Assessors ON Assessors.Id = AssessmentsAssessors.AssessorId
	                          WHERE Assessments.Assessed > DATEADD(Year,-1,GETDATE())
	                          AND Assessors.UserName = @Assessor
	                          GROUP BY ExamId
                          ) Assessments ON Exams.Id = Assessments.ExamId";
            }
        }
    }
}