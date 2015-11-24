using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Lisa.Excelsis.WebApi
{
    partial class Database
    {
        public object FetchAssessment(object id)
        {
            var query = @"SELECT Assessments.Id as [@], Assessments.Id, StudentName, StudentNumber, Assessed,
                                 Exams.Id as Exam_@Id, Exams.Name as Exam_Name, Exams.Cohort as Exam_Cohort, Exams.Crebo as Exam_Crebo, Exams.Subject as Exam_Subject,
                                 Assessors.Id as #Assessors_@Id, Assessors.UserName as #Assessors_UserName,
                                 Categories.Id as #Categories_@Id, Categories.Id as #Categories_Id, Categories.[Order] as #Categories_Order, Categories.Name as #Categories_Name,
                                 Observations.Id as #Categories_#Observations_@Id, Observations.Id as #Categories_#Observations_Id, Observations.Result as #Categories_#Observations_Result, Observations.Marks as #Categories_#Observations_Marks,
                                 Criteria.Id as #Categories_#Observations_Criterion_@Id, Criteria.Title as #Categories_#Observations_Criterion_Title, Criteria.Description as #Categories_#Observations_Criterion_Description, Criteria.[Order] as #Categories_#Observations_Criterion_Order, Criteria.Value as #Categories_#Observations_Criterion_Value
                          FROM Assessments
                          LEFT JOIN Exams ON Exams.Id = Assessments.Exam_Id
                          LEFT JOIN AssessmentsAssessors ON AssessmentsAssessors.Assessment_Id = Assessments.Id
                          LEFT JOIN Assessors ON Assessors.Id = AssessmentsAssessors.Assessor_Id                         
                          LEFT JOIN Observations ON Observations.Assessment_Id = Assessments.Id
                          LEFT JOIN Criteria ON Criteria.Id = Observations.Criterion_Id
                          LEFT JOIN Categories ON Categories.Id = Criteria.CategoryId
                          WHERE Assessments.Id = @Id";
            var parameters = new {
                Id = id
            };

            return _gateway.SelectSingle(query, parameters);
        }

        public IEnumerable<object> FetchAssessments(Filter filter)
        {
            List<string> queryList = new List<string>();

            var query = @"SELECT Assessments.Id as [@], Assessments.Id, StudentName, StudentNumber, Assessed,
                                 Exams.Id as Exam_@ID, Exams.Name as Exam_Name, Exams.Cohort as Exam_Cohort, Exams.Crebo as Exam_Crebo, Exams.Subject as Exam_Subject,
                                 Assessors.Id as #Assessors_@Id, Assessors.UserName as #Assessors_UserName
                          FROM Assessments
                          LEFT JOIN Exams ON Exams.Id = Assessments.Exam_Id
                          LEFT JOIN AssessmentsAssessors ON AssessmentsAssessors.Assessment_Id = Assessments.Id
                          LEFT JOIN Assessors ON Assessors.Id = AssessmentsAssessors.Assessor_Id";

            if (filter.Assessor != null)
            {
                queryList.Add( @" Assessments.Id IN(
                                      SELECT Assessments.Id
                                      FROM Assessments
                                      LEFT JOIN Exams ON Exams.Id = Assessments.Exam_Id
                                      LEFT JOIN AssessmentsAssessors ON AssessmentsAssessors.Assessment_Id = Assessments.Id
                                      LEFT JOIN Assessors ON Assessors.Id = AssessmentsAssessors.Assessor_Id
                                      WHERE Assessors.UserName = @Assessor
                                  )");
            }

            if (filter.StudentNumber != null)
            {
                queryList.Add(" Assessments.StudentNumber = @StudentNumber");
            }

            var parameters = new
            {
                Assessor = filter.Assessor ?? string.Empty,
                StudentNumber = filter.StudentNumber ?? string.Empty
            };

            query += (queryList.Count > 0) ? " WHERE " + string.Join(" AND ", queryList) : string.Join(" AND ", queryList);
            return _gateway.SelectMany(query, parameters);
        }

        public object AddAssessment(AssessmentPost assessment, string subject, string name, string cohort)
        {
            _errorMessages = new List<string>();

            dynamic examResult = FetchExam(subject, name, cohort);
            dynamic assessorResult = SelectAssessors(assessment);

            if (examResult != null && assessorResult != null && _errorMessages.Count == 0)
            {
                dynamic assessmentResult = InsertAssessment(assessment, examResult);
                InsertAssessmentAssessors(assessment, assessmentResult, assessorResult);
                InsertObservations(assessmentResult, examResult);

                return (_errorMessages.Count > 0) ? null : assessmentResult;
            }

            return null;
        }

        public void PatchAssessment(IEnumerable<Patch> patches, int id)
        {
            _errorMessages = new List<string>();
            foreach (Patch patch in patches)
            {
                switch (patch.Action)
                {
                    case "replace":
                        var fieldString = patch.Field.Split('/');
                        if(fieldString.ElementAt(0).ToLower() == "observations")
                        {
                            int observationId = Convert.ToInt32(fieldString.ElementAt(1));
                            PatchObservation(id, observationId, fieldString.ElementAt(2), patch.Value);
                        }
                        else
                        {
                            _errorMessages.Add("The fields you are trying to patch are not patchable.");
                        }
                        break;
                }
            }
        }

        private void PatchObservation(int id, int observationId, string field, object value)
        {
            if (field.ToLower() == "result" || field.ToLower() == "marks")
            {
                var query = @"UPDATE Observations
                              SET " + field + @" = @Value
                              WHERE Assessment_Id = @Id AND Id = @ObservationId";

                var parameters = new
                {
                    value = value,
                    Id = id,
                    ObservationId = observationId
                };

                _gateway.Insert(query, parameters);
            }
            else
            {
                _errorMessages.Add("The fields you are trying to patch are not patchable.");
            }
        }

        private dynamic SelectAssessors(AssessmentPost assessment)
        {
            var assessors = assessment.Assessors.Select(assessor => "'" + assessor.UserName + "'");

            var query = @"SELECT Id
                          FROM Assessors
                          WHERE UserName IN ( " + string.Join(",", assessors) + " ) ";
            dynamic result = _gateway.SelectMany(query);
            if (result.Count != assessment.Assessors.Count)
            {
                _errorMessages.Add("An assessor doesn't exist.");
            }

            return result;
        }

        private dynamic InsertAssessment(AssessmentPost assessment, dynamic examResult)
        {
            var query = @"INSERT INTO Assessments (StudentName, StudentNumber, Assessed, Exam_Id)
                          VALUES (@StudentName, @StudentNumber, @Assessed, @ExamId);";

            var parameters = new
            {
                StudentName = assessment.StudentName ?? string.Empty,
                StudentNumber = assessment.StudentNumber ?? string.Empty,
                Assessed = assessment.Assessed,
                ExamId = examResult.Id
            };

            return _gateway.Insert(query, parameters);
        }

        private void InsertObservations(dynamic assessmentResult, dynamic examResult)
        {
            List<string> observations = new List<string>();
            foreach(var category in examResult.Categories)
            {
                foreach(var criterion in category.Criteria)
                {
                    observations.Add("(" + criterion.Id + ", " + assessmentResult + ",'','')");
                }
            }
            
            var query = @"INSERT INTO Observations (Criterion_Id, Assessment_Id, Result, Marks) VALUES ";
            query += string.Join(",", observations);
            _gateway.Insert(query, null);
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