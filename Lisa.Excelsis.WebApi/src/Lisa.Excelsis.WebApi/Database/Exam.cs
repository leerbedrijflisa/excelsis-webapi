using System;
using System.Collections.Generic;
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
                        @" WHERE SubjectId = @Subject 
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
            _errors = new List<Error>();

            exam.Crebo = (exam.Crebo == null) ? string.Empty : exam.Crebo;

            var regexCrebo = new Regex(@"^$|^\d{5}$");
            if (!regexCrebo.IsMatch(exam.Crebo))
            {
                _errors.Add(new Error(1109, string.Format("The crebo number '{0}' doesn't meet the requirements of 5 digits.", exam.Crebo), new
                {
                    Crebo = exam.Crebo
                }));
            }

            var regexCohort = new Regex(@"^(19|20)\d{2}$");
            if (!regexCohort.IsMatch(exam.Cohort))
            {
                _errors.Add(new Error(1110, string.Format("The cohort year '{0}' doesn't meet the requirements of 4 digits.", exam.Cohort), new
                {
                    Cohort = exam.Cohort
                }));
            }

            string subjectId = CleanParam(exam.Subject);
            string nameId = CleanParam(exam.Name);

            if (subjectId == string.Empty)
            {
                _errors.Add(new Error(1103, "The 'subject' may not be empty.", new { field = "Subject"}));
            }

            if (nameId == string.Empty)
            {
                _errors.Add(new Error(1103, "The 'name' may not be empty.", new { field = "Name" }));
            }

            if (_errors.Count > 0)
            {
                return null;
            }

<<<<<<< HEAD
            var query = @"INSERT INTO Exams (Name, NameId, Cohort, Crebo, Subject, SubjectId)
                        VALUES (@Name, @NameId, @Cohort, @Crebo, @subject, @SubjectId);";
            var parameters = new
            {
                Name = exam.Name,
                NameId = nameId,
                Cohort = exam.Cohort,
                Crebo = exam.Crebo,
                Subject = exam.Subject,
                SubjectId = subjectId
            };
            return _gateway.Insert(query, parameters);
=======
            var query = @"INSERT INTO Exams (Name, Cohort, Crebo, Subject)
                        VALUES (@Name, @Cohort, @Crebo, @subject);";
            return _gateway.Insert(query, exam);
>>>>>>> feature/#23-change-criteria-post-to-exam-patch
        }

        public void PatchExam(IEnumerable<Patch> patches, int id)
        {
            _errors = new List<Error>();
            foreach (Patch patch in patches)
            {
                patch.Action.ToLower();
                patch.Field.ToLower();
                var field = patch.Field.Split('/');

                switch (patch.Action)
                {
                    case "add":
                        if (Regex.IsMatch(patch.Field, @"^categories/\d+/criteria*$"))
                        {
                            if (CategoryExists(id, Convert.ToInt32(field[1])))
                            {
                                AddCriterion(id, Convert.ToInt32(field[1]), patch);
                            }
                            else
                            {
                                _errors.Add(new Error(0, string.Format("The category with id '{0}' doesn't exist.", Convert.ToInt32(field[1])), new { id = Convert.ToInt32(field[1]) }));
                            }
                        }
                        else
                        {
                            _errors.Add(new Error(0, string.Format("The field '{0}' is not patchable.", patch.Field), new { field = patch.Field }));
                        }
                        break;
                    case "replace":
                        break;
                    case "remove":
                        break;
                    default:
                        _errors.Add(new Error(0, string.Format("The action '{0}' doesn't exist.", patch.Action), new { action = patch.Action }));
                        break;
                }
            }
        }
        public bool ExamExists(ExamPost exam)
        {
            _errors = new List<Error>();
            string subject = CleanParam(exam.Subject);
            string name = CleanParam(exam.Name);
            exam.Crebo = (exam.Crebo == null)? string.Empty : exam.Crebo;

            var query = @"SELECT COUNT(*) as count FROM Exams
                          WHERE NameId = @Name
                            AND SubjectId = @Subject
                            AND Cohort = @Cohort
                            AND Crebo = @Crebo";

            var parameters = new
            {
                Name = name,
                Subject = subject,
                Cohort = exam.Cohort,
                Crebo = exam.Crebo
            };
            dynamic result = _gateway.SelectSingle(query, parameters);

            return (result.count > 0);
        }

        public bool ExamExists(int id)
        {
            _errors = new List<Error>();

            var query = @"SELECT COUNT(*) as count FROM Exams
                          WHERE Id = @Id";
            dynamic result = _gateway.SelectSingle(query, new { Id = id });

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