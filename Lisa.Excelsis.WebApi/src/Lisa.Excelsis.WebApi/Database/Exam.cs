using System;
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
                Assessor = filter.Assessors ?? string.Empty
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
                Assessor = filter.Assessors ?? string.Empty
            };

            return _gateway.SelectMany(query, parameters);
        }

        public object AddExam(ExamPost exam)
        {
            _errors = new List<Error>();
            string subjectId = CleanParam(exam.Subject);
            string nameId = CleanParam(exam.Name);
            exam.Crebo = (exam.Crebo == null) ? string.Empty : exam.Crebo;

            if (!Regex.IsMatch(exam.Crebo, @"^$|^\d{5}$"))
            {
                _errors.Add(new Error(1203, new { field = "crebo", value = exam.Crebo, count = 5 }));
            }

            if (!Regex.IsMatch(exam.Cohort, @"^(19|20)\d{2}$"))
            {
                _errors.Add(new Error(1207, new { field = "cohort", value = exam.Cohort, count = 4 , min = 1900, max = 2099 }));
            }

            if (subjectId == string.Empty)
            {
                _errors.Add(new Error(1206, new { field = "Subject", value = exam.Subject }));
            }

            if (nameId == string.Empty)
            {
                _errors.Add(new Error(1206, new { field = "Name", value = exam.Name }));
            }

            if (_errors.Any())
            {
                return null;
            }

            var query = @"INSERT INTO Exams (Name, NameId, Cohort, Crebo, Subject, SubjectId, Status)
                        VALUES (@Name, @NameId, @Cohort, @Crebo, @subject, @SubjectId, 'draft');";
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
        }

        public void PatchExam(IEnumerable<Patch> patches, int id)
        {
            _errors = new List<Error>();
            foreach (Patch patch in patches)
            {
                if(patch.Action != null)
                {
                    patch.Action.ToLower();
                    if (patch.Field != null)
                    {
                        patch.Field.ToLower();
                        var field = patch.Field.Split('/');
                        

                        switch (patch.Action)
                        {
                            case "add":// patch.field = categories/{digit}/criteria
                                if (patch.Value != null)
                                {
                                    if (Regex.IsMatch(patch.Field, @"^categories/\d+/criteria$"))
                                    {
                                        if (CategoryExists(id, field[1]))
                                        {
                                            AddCriterion(id, field[1], patch);
                                        }
                                        else
                                        {
                                            _errors.Add(new Error(1300, new { field = "category", value = field[1] }));
                                        }
                                    }
                                    else if (Regex.IsMatch(patch.Field, @"^categories$"))
                                    {
                                        AddCategory(id, patch);
                                    }
                                    else
                                    {
                                        _errors.Add(new Error(1205, new { field = "field", value = patch.Field }));
                                    }
                                } 
                                else
                                {
                                    _errors.Add(new Error(1101, new { field = "value" }));
                                }
                                break;
                            case "replace":
                                if (patch.Value != null)
                                {
                                    if (Regex.IsMatch(patch.Field, @"^status$"))
                                    {
                                        if (ExamExists(id))
                                        {
                                            ReplaceStatus(id, patch.Field, patch.Value.ToString());
                                        }
                                        else
                                        {
                                            _errors.Add(new Error(1300, new { field = "Exam", value = id }));
                                        }
                                    }
                                }
                                else
                                {
                                    _errors.Add(new Error(1101, new { field = "value" }));
                                }
                                break;
                            case "move":
                                if(patch.Target != null)
                                {
                                    var target = patch.Target.Split('/');
                                    if (Regex.IsMatch(patch.Field, @"^categories/\d+/criteria/\d+$"))
                                    {
                                        if (Regex.IsMatch(patch.Target, @"^categories/\d+$"))
                                        {
                                            if (CategoryExists(id, field[1]))
                                            {
                                                if (CategoryExists(id, target[1]))
                                                {
                                                    if (CriterionExists(id, field[1],field[3]))
                                                    {
                                                        MoveCriterion(id, field[3], target[1]);
                                                    }
                                                    else
                                                    {
                                                        _errors.Add(new Error(1300, new { field = "criterion", value = field[3] }));
                                                    }
                                                }
                                                else
                                                {
                                                    _errors.Add(new Error(1300, new { field = "category", value = target[1] }));
                                                }
                                            }
                                            else
                                            {
                                                _errors.Add(new Error(1300, new { field = "category", value = field[1] }));
                                            }
                                        }
                                        else
                                        {
                                            _errors.Add(new Error(1205, new { field = "target", value = patch.Target }));
                                        }
                                    }
                                    else
                                    {
                                        _errors.Add(new Error(1205, new { field = "field", value = patch.Field }));
                                    }
                                }
                                else
                                {
                                    _errors.Add(new Error(1101, new { field = "target" }));
                                }
                                break;
                            case "remove":
                                break;
                            default:
                                _errors.Add(new Error(1303, new { value = patch.Action }));
                                break;
                        }
                    }
                    else
                    {
                        _errors.Add(new Error(1101, new { field = "field" }));
                    }
                }
                else
                {
                    _errors.Add(new Error(1101, new { field = "action" }));
                }
            }
        }

        public void ReplaceStatus(int id, string field, string value)
        {
            string valueLower = value.ToLower();
            if (valueLower != "draft" && valueLower != "published")
            {
                _errors.Add(new Error(1204, new { field = "status", value = value.ToString(), permitted = new string[] { "draft", "published" } }));
            }

            var query = @"UPDATE Exams SET status = @value
                          WHERE Exams.id = @id";

            var parameters = new
            {
                id = id,
                value = value
            };
            _gateway.Update(query, parameters);
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
                Name = CleanParam(exam.Name),
                Subject = CleanParam(exam.Subject),
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