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
                                 Assessments.Name as Exam_@Name, Assessments.Name as Exam_Name, Assessments.Cohort as Exam_Cohort, Assessments.Crebo as Exam_Crebo, Assessments.Subject as Exam_Subject,
                                 Assessors.Id as #Assessors_@Id, Assessors.UserName as #Assessors_UserName, Assessors.Firstname as #Assessors_FirstName, Assessors.LastName as #Assessors_LastName,
                                 AssessmentCategories.Id as #Categories_@Id, AssessmentCategories.Id as #Categories_Id, AssessmentCategories.[Order] as #Categories_Order, AssessmentCategories.Name as #Categories_Name,
                                 Observations.Id as #Categories_#Observations_@Id, Observations.Id as #Categories_#Observations_Id, Observations.Result as #Categories_#Observations_Result,
                                 Marks.Id as #Categories_#Observations_#Marks_@Id, Marks.Name as #Categories_#Observations_#Marks_Name,
                                 Observations.[Order] as #Categories_#Observations_Criterion_@Order, Observations.Title as #Categories_#Observations_Criterion_Title, Observations.Description as #Categories_#Observations_Criterion_Description, Observations.[Order] as #Categories_#Observations_Criterion_Order, Observations.Weight as #Categories_#Observations_Criterion_Weight
                          FROM Assessments
                          LEFT JOIN AssessmentAssessors ON AssessmentAssessors.AssessmentId = Assessments.Id
                          LEFT JOIN Assessors ON Assessors.Id = AssessmentAssessors.AssessorId
                          LEFT JOIN Observations ON Observations.AssessmentId = Assessments.Id
                          LEFT JOIN Marks ON Marks.ObservationId = Observations.Id                         
                          LEFT JOIN AssessmentCategories ON AssessmentCategories.Id = Observations.CategoryId
                          WHERE Assessments.Id = @Id";

            if (Startup.Profile.IsInRole("student"))
            {
                query += " AND Assessments.StudentNumber = @Profile";
            }

            dynamic result = _gateway.SelectSingle(query, new { Id = id, Profile = Startup.Profile.Number });
                       
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
                                 Assessments.Name as Exam_@Name, Assessments.Name as Exam_Name, Assessments.Cohort as Exam_Cohort, Assessments.Crebo as Exam_Crebo, Assessments.Subject as Exam_Subject,
                                 Assessors.Id as #Assessors_@Id, Assessors.UserName as #Assessors_UserName, Assessors.Firstname as #Assessors_FirstName, Assessors.LastName as #Assessors_LastName
                          FROM Assessments
                          LEFT JOIN AssessmentAssessors ON AssessmentAssessors.AssessmentId = Assessments.Id
                          LEFT JOIN Assessors ON Assessors.Id = AssessmentAssessors.AssessorId";

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
                                              LEFT JOIN Exams ON Exams.Id = Assessments.ExamId
                                              LEFT JOIN AssessmentAssessors ON AssessmentAssessors.AssessmentId = Assessments.Id
                                              LEFT JOIN Assessors ON Assessors.Id = AssessmentAssessors.AssessorId
                                              WHERE " + assessorQuery + ")");
            }

            if (filter.Student != null)
            {
                assessmentQueryList.Add(" Assessments.StudentNumber LIKE @Student OR  Assessments.StudentName LIKE @Student");
            }

            if(Startup.Profile.IsInRole("student"))
            {
                assessmentQueryList.Add(" Assessments.StudentNumber = @Profile");
            }

            var parameters = new
            {
                Assessor = filter.Assessors ?? string.Empty,
                Student = filter.Student ?? string.Empty,
                Profile = Startup.Profile.Number
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

        public object FetchAssessmentsByExam(object id)
        {
            var query = @"SELECT Assessments.Id as [@], Assessments.Id, StudentNumber as Student_@, StudentName as Student_Name, StudentNumber as Student_Number, Assessed,
                                 Assessments.Name as Exam_@Name, Assessments.Name as Exam_Name, Assessments.Cohort as Exam_Cohort, Assessments.Crebo as Exam_Crebo, Assessments.Subject as Exam_Subject,
                                 Assessors.Id as #Assessors_@Id, Assessors.UserName as #Assessors_UserName, Assessors.Firstname as #Assessors_FirstName, Assessors.LastName as #Assessors_LastName
                          FROM Assessments
                          LEFT JOIN AssessmentAssessors ON AssessmentAssessors.AssessmentId = Assessments.Id
                          LEFT JOIN Assessors ON Assessors.Id = AssessmentAssessors.AssessorId
                          WHERE Assessments.ExamId = @Id";
            return _gateway.SelectMany(query, new { Id = id});
        }
        public object AddAssessment(AssessmentPost assessment, string subject, string name, string cohort, dynamic examResult)
        {
            object assessorResult = SelectAssessors(assessment.Assessors);            
            object assessmentResult = InsertAssessment(assessment, examResult);

            AddAssessmentAssessors(assessment, assessmentResult, assessorResult);
            AddCategories(assessmentResult, examResult);
            AddObservations(assessmentResult, examResult);

            return assessmentResult;
        }

        public void PatchAssessment(IEnumerable<Patch> patches, int id)
        {
            AssessmentBuilder builder = new AssessmentBuilder();
            builder.BuildPatches(id, patches);
        }
        
        public bool AssessmentExists(object id)
        {
            var query = @"SELECT COUNT(*) as count FROM Assessments
                          WHERE Id = @Id";
            dynamic result = _gateway.SelectSingle(query, new { Id = id });

            return (result.count > 0);
        }

        public object SelectAssessors(string[] assessors)
        {
            var query = @"SELECT Id, UserName FROM Assessors
                        WHERE UserName IN ( @Assessors ) ";
            dynamic result = _gateway.SelectMany(query, new { Assessors = assessors });
            return result;
        }

        private object InsertAssessment(AssessmentPost assessment, dynamic examResult)
        {
            var query = @"INSERT INTO Assessments (ExamId, StudentName, StudentNumber, Assessed, Name, Cohort, Crebo, Subject)
                          VALUES (@ExamId, @StudentName, @StudentNumber, @Assessed, @Name, @Cohort, @Crebo, @Subject);";
            
            var parameters = new
            {
                ExamId = examResult.Id,
                StudentName = assessment.Student?.Name ?? string.Empty,
                StudentNumber = assessment.Student?.Number ?? string.Empty,
                Assessed = assessment.Assessed?.ToString("s") + "Z",
                Name = examResult.Name,
                Cohort = examResult.Cohort,
                Crebo = examResult.Crebo,
                Subject = examResult.Subject
            };

            return _gateway.Insert(query, parameters);
        }

        private void AddAssessmentAssessors(AssessmentPost assessment, dynamic assessmentResult, dynamic assessorResult)
        {
            foreach(dynamic assessor in assessorResult)
            {
                var query = @"INSERT INTO AssessmentAssessors (AssessmentId, AssessorId) 
                              VALUES (@Assessment, @Assessor)";
                _gateway.Insert(query, new { Assessment = assessmentResult, Assessor = assessor.Id });
            }
        }        
    }
}