using System;
using System.Collections.Generic;

namespace Lisa.Excelsis.WebApi
{
    partial class Database
    {
        public object AddAssessment(AssessmentPost assessment, string subject, string name, string cohort)
        {
            var query = @"SELECT Id FROM Exams WHERE Subject = @Subject AND Name = @Name AND Cohort = @Cohort";
            var parameters = new { Subject = subject, Name = name, Cohort = cohort };
            dynamic examResult = _gateway.SelectSingle(query, parameters);

            if (examResult == null)
            {
                return new { error = 422 };
            }

            query = @"INSERT INTO Assessments (StudentName, StudentNumber, Assessed, Exam_Id)
                          VALUES (@StudentName, @StudentNumber, @Assessed, @ExamId);";

            if (assessment.StudentName == null)
            {
                assessment.StudentName = string.Empty;
            }
            if(assessment.StudentNumber == null)
            {
                assessment.StudentNumber = string.Empty;
            }

            var parameters2 = new { StudentName = assessment.StudentName, StudentNumber = assessment.StudentNumber, Assessed = assessment.Assessed, ExamId = examResult.Id };
            dynamic assessmentResult = _gateway.Insert(query, parameters2);

            string assessors = string.Empty;
            foreach (var assessor in assessment.Assessors)
            {
                assessors += ",'" + assessor.UserName + "'";
            }

            query = @"SELECT Id From Assessors WHERE UserName IN ( " + assessors.Substring(1) + " ) ";
            dynamic result = _gateway.SelectMany(query);

            if (result.Count != assessment.Assessors.Count)
            {
                return new { error = 422 };
            }

            query = @"INSERT INTO AssessmentsAssessors (Assessment_Id, Assessor_Id) VALUES ";
            string[] AssessorString = new string[assessment.Assessors.Count];
            int i = 0;
            foreach(var assessor in result)
            {                
                AssessorString[i] = "(" + assessmentResult + ", " + assessor.Id + ")";
                i++;
            }
            query += string.Join(",", AssessorString);
            _gateway.Insert(query, null);

            return assessmentResult;
        }        
        public object FetchAssessment(int id)
        {
            var query = @"SELECT Assessments.Id as [@], Assessments.Id, StudentName, StudentNumber, Assessed, 
                                 Exams.Id as Exams_@ID, Exams.Name as Exams_Name, Exams.Cohort as Exams_Cohort, Exams.Crebo as Exams_Crebo, Exams.Subject as Exams_Subject
                          FROM Assessments
                          LEFT JOIN Exams ON Exams.Id = Assessments.Exam_Id
                          WHERE Assessments.Id = @Id";
            var parameters = new { Id = id };
            return _gateway.SelectSingle(query, parameters);
        }

        public IEnumerable<object> FetchAssessments()
        {
            var query = @"SELECT Assessments.Id as [@], Assessments.Id, StudentName, StudentNumber, Assessed, 
                                 Exams.Id as Exams_@ID, Exams.Name as Exams_Name, Exams.Cohort as Exams_Cohort, Exams.Crebo as Exams_Crebo, Exams.Subject as Exams_Subject
                          FROM Assessments
                          LEFT JOIN Exams ON Exams.Id = Assessments.Exam_Id";
            return _gateway.SelectMany(query);
        }
    }
}
