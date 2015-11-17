using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Lisa.Excelsis.WebApi
{
    partial class Database
    {
        public object AddAssessment(AssessmentPost assessment, string subject, string name, string cohort)
        {
            _errorMessage = new List<string>();

            dynamic examResult = SelectExam(subject, name, cohort);

            dynamic assessorResult = SelectAssessors(assessment);

            if (examResult != null && assessorResult != null && _errorMessage.Count == 0)
            {
                dynamic assessmentResult = InsertAssessment(assessment, examResult);

                InsertAssessmentAssessors(assessment, assessmentResult, assessorResult);

                InsertObservations(assessmentResult, examResult);

                return (_errorMessage.Count > 0) ? null : assessmentResult;
            }

            return null;            
        }   
             
        public object FetchAssessment(int id)
        {
            var query = @"SELECT Assessments.Id as [@], Assessments.Id, StudentName, StudentNumber, Assessed, 
                                 Exams.Id as Exams_@Id, Exams.Name as Exams_Name, Exams.Cohort as Exams_Cohort, Exams.Crebo as Exams_Crebo, Exams.Subject as Exams_Subject,
                                 Assessors.Id as #Assessors_@Id, Assessors.UserName as #Assessors_UserName,
                                 Observations.Id as #Observations_Id, Observations.Result as #Observations_Result, Observations.Marks as #Observations_Marks,
                                 Criteriums.Id as #Observations_Criterium_@Id, Criteriums.Title as #Observations_Criterium_Title, Criteriums.Description as #Observations_Criterium_Description, Criteriums.[Order] as #Observations_Criterium_Order, Criteriums.Value as #Observations_Criterium_Value
                          FROM Assessments
                          LEFT JOIN Exams ON Exams.Id = Assessments.Exam_Id
                          LEFT JOIN AssessmentsAssessors ON AssessmentsAssessors.Assessment_Id = Assessments.Id
                          LEFT JOIN Assessors ON Assessors.Id = AssessmentsAssessors.Assessor_Id 
                          LEFT JOIN Observations ON Observations.Assessment_Id = Assessments.Id
                          LEFT JOIN Criteriums ON Criteriums.Id = Observations.Criterium_Id
                          WHERE Assessments.Id = @Id";
            var parameters = new { Id = id };
            return _gateway.SelectSingle(query, parameters);
        }

        public IEnumerable<object> FetchAssessments()
        {
            var query = @"SELECT Assessments.Id as [@], Assessments.Id, StudentName, StudentNumber, Assessed, 
                                 Exams.Id as Exams_@ID, Exams.Name as Exams_Name, Exams.Cohort as Exams_Cohort, Exams.Crebo as Exams_Crebo, Exams.Subject as Exams_Subject,
                                 Assessors.Id as #Assessors_@Id, Assessors.UserName as #Assessors_UserName
                          FROM Assessments
                          LEFT JOIN Exams ON Exams.Id = Assessments.Exam_Id
                          LEFT JOIN AssessmentsAssessors ON AssessmentsAssessors.Assessment_Id = Assessments.Id
                          LEFT JOIN Assessors ON Assessors.Id = AssessmentsAssessors.Assessor_Id "
;
            return _gateway.SelectMany(query);
        }

        private dynamic SelectExam(string subject, string name, string cohort)
        {
            var query = @"SELECT Exams.Id as [@], Exams.Id, Criteriums.Id as #Criterium_Id FROM Exams 
                          LEFT JOIN Criteriums ON Criteriums.ExamId = Exams.Id
                          WHERE Subject = @Subject AND Name = @Name AND Cohort = @Cohort";
            var parameters = new { Subject = subject, Name = name, Cohort = cohort };
            dynamic result = _gateway.SelectSingle(query, parameters);

            if (result == null)
            {
                _errorMessage.Add("Exam doensn't exist.");
            }

            return result;
        }

        private dynamic SelectAssessors(AssessmentPost assessment)
        {          
            var assessors = assessment.Assessors.Select(assessor => "'" + assessor.UserName + "'");

            var query = @"SELECT Id From Assessors WHERE UserName IN ( " + string.Join(",", assessors) + " ) ";
            dynamic result = _gateway.SelectMany(query);

            if (result.Count != assessment.Assessors.Count)
            {
                _errorMessage.Add("An assessor doensn't exist.");
            }

            return result;
        }

        private dynamic InsertAssessment(AssessmentPost assessment, dynamic examResult)
        {
            var query = @"INSERT INTO Assessments (StudentName, StudentNumber, Assessed, Exam_Id)
                          VALUES (@StudentName, @StudentNumber, @Assessed, @ExamId);";

            if (assessment.StudentName == null)
            {
                assessment.StudentName = string.Empty;
            }
            if (assessment.StudentNumber == null)
            {
                assessment.StudentNumber = string.Empty;
            }

            var parameters = new { StudentName = assessment.StudentName, StudentNumber = assessment.StudentNumber, Assessed = assessment.Assessed, ExamId = examResult.Id };
            return _gateway.Insert(query, parameters);
        }

        private void InsertObservations(dynamic assessmentResult, dynamic examResult)
        {
            var observations = ((IEnumerable)examResult.Criterium).Cast<dynamic>().Select(criterium => "(" + criterium.Id + ", " + assessmentResult + ",'','')");
           
            var query = @"INSERT INTO Observations (Criterium_Id, Assessment_Id, Result, Marks) VALUES ";
            query += string.Join(",", observations);
            _gateway.Insert(query, null);
        }

        private void InsertAssessmentAssessors(AssessmentPost assessment, dynamic assessmentResult, dynamic assessorResult)
        {
            var assessorAssessments = ((IEnumerable)assessorResult).Cast<dynamic>().Select(assessor => "(" + assessmentResult + ", " + assessor.Id + ")");
            
            var query = @"INSERT INTO AssessmentsAssessors (Assessment_Id, Assessor_Id) VALUES ";
            query += string.Join(",", assessorAssessments);
            _gateway.Insert(query, null);
        }

        private List<string> _errorMessage { get; set; }
    }
}