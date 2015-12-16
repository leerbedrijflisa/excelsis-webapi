using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Lisa.Excelsis.WebApi
{
    partial class Database
    {
        public object FetchAssessment(object id)
        {
            var query = @"SELECT Assessments.Id as [@], Assessments.Id, StudentNumber as Student_@Number, StudentName as Student_Name, StudentNumber as Student_Number, Assessed,
                                 Exams.Id as Exam_@Id, Exams.Name as Exam_Name, Exams.Cohort as Exam_Cohort, Exams.Crebo as Exam_Crebo, Exams.Subject as Exam_Subject,
                                 Assessors.Id as #Assessors_@Id, Assessors.UserName as #Assessors_UserName,
                                 Categories.Id as #Categories_@Id, Categories.Id as #Categories_Id, Categories.[Order] as #Categories_Order, Categories.Name as #Categories_Name,
                                 Observations.Id as #Categories_#Observations_@Id, Observations.Id as #Categories_#Observations_Id, Observations.Result as #Categories_#Observations_Result,
                                 Marks.Id as #Categories_#Observations_#Marks_@Id, Marks.Name as #Categories_#Observations_#Marks_Name,
                                 Criteria.Id as #Categories_#Observations_Criterion_@Id, Criteria.Title as #Categories_#Observations_Criterion_Title, Criteria.Description as #Categories_#Observations_Criterion_Description, Criteria.[Order] as #Categories_#Observations_Criterion_Order, Criteria.Weight as #Categories_#Observations_Criterion_Weight
                          FROM Assessments
                          LEFT JOIN Exams ON Exams.Id = Assessments.Exam_Id
                          LEFT JOIN AssessmentsAssessors ON AssessmentsAssessors.Assessment_Id = Assessments.Id
                          LEFT JOIN Assessors ON Assessors.Id = AssessmentsAssessors.Assessor_Id
                          LEFT JOIN Observations ON Observations.Assessment_Id = Assessments.Id
                          LEFT JOIN Marks ON Marks.Observation_Id = Observations.Id
                          LEFT JOIN Criteria ON Criteria.Id = Observations.Criterion_Id
                          LEFT JOIN Categories ON Categories.Id = Criteria.CategoryId
                          WHERE Assessments.Id = @Id";
            var parameters = new {
                Id = id
            };

            dynamic result = _gateway.SelectSingle(query, parameters);
            if (result == null)
            {
                return null;
            }

            foreach(dynamic category in result.Categories)
            {
                foreach(dynamic observation in category.Observations)
                {
                    List<string> marks = new List<string>();
                    foreach (dynamic mark in observation.Marks)
                    {
                        marks.Add(mark.Name);
                    }
                    observation.Marks = marks.GroupBy(m => m).Select(g => g.First()).ToArray();
                }
            }
            return result;
        }

        public IEnumerable<object> FetchAssessments(Filter filter)
        {
            bool multipleAssessors = false;
            List<string> assessmentQueryList = new List<string>();
            string assessorQuery = string.Empty;

            var query = @"SELECT Assessments.Id as [@], Assessments.Id, StudentNumber as Student_@, StudentName as Student_Name, StudentNumber as Student_Number, Assessed,
                                 Exams.Id as Exam_@ID, Exams.Name as Exam_Name, Exams.Cohort as Exam_Cohort, Exams.Crebo as Exam_Crebo, Exams.Subject as Exam_Subject,
                                 Assessors.Id as #Assessors_@Id, Assessors.UserName as #Assessors_UserName
                          FROM Assessments
                          LEFT JOIN Exams ON Exams.Id = Assessments.Exam_Id
                          LEFT JOIN AssessmentsAssessors ON AssessmentsAssessors.Assessment_Id = Assessments.Id
                          LEFT JOIN Assessors ON Assessors.Id = AssessmentsAssessors.Assessor_Id";

            if (filter.Assessors != null)
            {
                if (Regex.IsMatch(filter.Assessors, @"^[a-zA-Z]*$"))
                {
                    assessorQuery = @" Assessors.UserName = '" + filter.Assessors + "'";
                }
                else if(Regex.IsMatch(filter.Assessors, @"^([ |A-Za-z]+)$"))
                {
                    string assessors = string.Join(",", Regex.Split(filter.Assessors, @" "));
                    assessorQuery = @" Assessors.UserName IN('" + assessors.Replace(",", "','") + "')";
                    multipleAssessors = true;
                }
                else if(Regex.IsMatch(filter.Assessors, @"^([,|A-Za-z]+)$"))
                {
                    assessorQuery = @" Assessors.UserName IN('" + filter.Assessors.Replace(",", "','") + "')";
                }
                assessmentQueryList.Add(@" Assessments.Id IN(
                                              SELECT Assessments.Id
                                              FROM Assessments
                                              LEFT JOIN Exams ON Exams.Id = Assessments.Exam_Id
                                              LEFT JOIN AssessmentsAssessors ON AssessmentsAssessors.Assessment_Id = Assessments.Id
                                              LEFT JOIN Assessors ON Assessors.Id = AssessmentsAssessors.Assessor_Id
                                              WHERE " + assessorQuery + ")");
            }

            if (filter.Student != null)
            {
                assessmentQueryList.Add(" Assessments.StudentNumber = @StudentNumber");
            }

            var parameters = new
            {
                Assessor = filter.Assessors ?? string.Empty,
                StudentNumber = filter.Student ?? string.Empty
            };
           
            query += (assessmentQueryList.Count > 0) ? " WHERE " + string.Join(" AND ", assessmentQueryList) : string.Join(" AND ", assessmentQueryList);
            dynamic results = _gateway.SelectMany(query, parameters);
            List<dynamic> newResults = new List<dynamic>();
            if (multipleAssessors)
            {
                string[] assessorString = filter.Assessors.Split(' ');
                foreach (var assessment in results)
                {
                    int count = 0;
                    foreach(var assessor in assessment.Assessors)
                    {
                        int pos = Array.IndexOf(assessorString, assessor.UserName);
                        if (pos > -1) count++;
                    }
                    if (count == assessorString.Count()) newResults.Add(assessment);
                }
                return newResults;
            }
            return results;
        }

        public object AddAssessment(AssessmentPost assessment, string subject, string name, string cohort, dynamic examResult)
        {
            _errors = new List<Error>();
            if (assessment.Student != null)
            {
                if (assessment.Student.Name != null && !Regex.IsMatch(assessment.Student.Name, @"^\s*(\w+\s)*\w+\s*$"))
                {
                    _errors.Add(new Error(1201, new { field = "studentname", value = assessment.Student.Name }));
                }

                if (assessment.Student.Number != null && !Regex.IsMatch(assessment.Student.Number, @"^\d{8}$"))
                {
                    _errors.Add(new Error(1203, new { field = "studentnumber", value = assessment.Student.Number, count = 8 }));
                }
            }
            else
            {
                assessment.Student = new Student();
            }

            object assessorResult = SelectAssessors(assessment);

            if (_errors.Count() == 0)
            {
                object assessmentResult = InsertAssessment(assessment, examResult);
                InsertAssessmentAssessors(assessment, assessmentResult, assessorResult);
                AddObservations(assessmentResult, examResult);

                return (_errors.Count() > 0) ? null : assessmentResult;
            }

            return null;
        }

        public void PatchAssessment(IEnumerable<Patch> patches, int id)
        {
            _errors = new List<Error>();
            foreach (Patch patch in patches)
            {
                if (patch.Action != null)
                {
                    patch.Action.ToLower();
                    if (patch.Field != null)
                    {
                        patch.Field.ToLower();
                        var field = patch.Field.Split('/');
                        if (patch.Value != null)
                        {
                            switch (patch.Action)
                            {
                                case "add":
                                    if (Regex.IsMatch(patch.Field, @"^observations/\d+/marks$"))
                                    {
                                        if (ObservationExists(id, Convert.ToInt32(field[1])))
                                        {
                                            AddMark(patch);
                                        }
                                        else
                                        {
                                            _errors.Add(new Error(1300, new { field = "observation", value = field[1] }));
                                        }
                                    }
                                    else
                                    {
                                        _errors.Add(new Error(1205, new { field = "field", value = patch.Field }));
                                    }
                                    break;
                                case "replace":
                                    if (Regex.IsMatch(patch.Field, @"^observations/\d+/result$"))
                                    {
                                        if (ObservationExists(id, Convert.ToInt32(field[1])))
                                        {
                                            ReplaceResult(id, patch);
                                        }
                                        else
                                        {
                                            _errors.Add(new Error(1300, new { field = "observation", value = field[1] }));
                                        }
                                    }
                                    else if (Regex.IsMatch(patch.Field, @"^(studentnumber|studentname)$"))
                                    {
                                        ReplaceStudent(id, patch);
                                    }
                                    else
                                    {
                                        _errors.Add(new Error(1205, new { field = "field", value = patch.Field }));
                                    }
                                    break;
                                case "remove":
                                    if (Regex.IsMatch(patch.Field, @"^observations/\d+/marks$"))
                                    {
                                        if (ObservationExists(id, Convert.ToInt32(field[1])))
                                        {
                                            RemoveMark(patch);
                                        }
                                        else
                                        {
                                            _errors.Add(new Error(1300, new { field = "observation", value = field[1] }));
                                        }
                                    }
                                    else
                                    {
                                        _errors.Add(new Error(1205, new { field = "field", value = patch.Field }));
                                    }
                                    break;
                                default:
                                    _errors.Add(new Error(1303, new { value = patch.Action }));
                                    break;
                            }
                        }
                        else
                        {
                            _errors.Add(new Error(1101, new { field = "value" }));
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

        private void ReplaceStudent(int id, Patch patch)
        {
            if (Regex.IsMatch(patch.Field, @"^studentname$") && !Regex.IsMatch(patch.Value.ToString().ToLower(), @"^[a-zA-Z\s]*$"))
            { 
                _errors.Add(new Error(1201, new { field = "studentname", value = patch.Value.ToString() }));
            }
            else if (Regex.IsMatch(patch.Field, @"^studentnumber$") && !Regex.IsMatch(patch.Value.ToString().ToLower(), @"^\d{8}$"))
            {
                _errors.Add(new Error(1203, new { field = "studentnumber", value = patch.Value.ToString(), count = 8 }));
            }

            if(_errors.Count == 0)
            {
                var query = @"UPDATE Assessments
                              SET " + patch.Field + @" = @Value
                              WHERE Id = @Id";
                var parameters = new
                {
                    value = patch.Value.ToString(),
                    Id = id
                };
                _gateway.Update(query, parameters);
            }
        }

        public bool AssessmentExists(int id)
        {
            var query = @"SELECT COUNT(*) as count FROM Assessments
                          WHERE Id = @id";
            var parameters = new
            {
               Id = id
            };
            dynamic result = _gateway.SelectSingle(query, parameters);

            return (result.count > 0);
        }

        private object SelectAssessors(AssessmentPost assessment)
        {
            if (assessment.Assessors.Any())
            {
                var assessors = assessment.Assessors.Select(assessor => "'" + assessor + "'");

                var query = @"SELECT Id, UserName
                          FROM Assessors
                          WHERE UserName IN ( " + string.Join(",", assessors) + " ) ";
                dynamic result = _gateway.SelectMany(query);


                if (result.Count != assessment.Assessors.Count())
                {
                    foreach (var assessor in assessment.Assessors)
                    {
                        if (result.Count == 0 || (result.Count > 0 && !((IEnumerable<dynamic>)result).Any(a => a.UserName == assessor)))
                        {
                            _errors.Add(new Error(1302, new { value = assessor }));
                        }
                    }
                }

                return result;
            }
            _errors.Add(new Error(1101, new { field = "assessors"}));
            return null;
        }

        private object InsertAssessment(AssessmentPost assessment, dynamic examResult)
        {
            var query = @"INSERT INTO Assessments (StudentName, StudentNumber, Assessed, Exam_Id)
                          VALUES (@StudentName, @StudentNumber, @Assessed, @ExamId);";

            var parameters = new
            {
                StudentName = assessment.Student.Name ?? string.Empty,
                StudentNumber = assessment.Student.Number ?? string.Empty,
                Assessed = assessment.Assessed,
                ExamId = examResult.Id
            };

            return _gateway.Insert(query, parameters);
        }

        private void InsertAssessmentAssessors(AssessmentPost assessment, dynamic assessmentResult, dynamic assessorResult)
        {
            var assessorAssessments = ((IEnumerable<dynamic>)assessorResult).Select(assessor => "(" + assessmentResult + ", " + assessor.Id + ")");

            var query = @"INSERT INTO AssessmentsAssessors (Assessment_Id, Assessor_Id) VALUES ";
            query += string.Join(",", assessorAssessments);
            _gateway.Insert(query, null);
        }
    }
}