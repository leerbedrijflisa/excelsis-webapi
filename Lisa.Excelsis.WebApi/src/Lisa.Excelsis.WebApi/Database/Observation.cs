using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Lisa.Excelsis.WebApi
{
    partial class Database
    {
        public void AddObservations(dynamic assessmentResult, dynamic examResult)
        {
            List<string> observations = new List<string>();
            if (examResult.Categories.Count > 0)
            {
                foreach (var category in examResult.Categories)
                {
                    foreach (var criterion in category.Criteria)
                    {
                        observations.Add("(" + criterion.Id + ", " + assessmentResult + ",'')");
                    }
                }

                var query = @"INSERT INTO Observations (CriterionId, AssessmentId, Result) VALUES ";
                query += string.Join(",", observations);
                _gateway.Insert(query, null);
            }
        }
        
        private bool ObservationExists(int assessmentId, int id)
        {
            var query = @"SELECT COUNT(*) as count FROM Observations
                          WHERE AssessmentId = @AssessmentId AND Id = @id";
            var parameters = new { AssessmentId = assessmentId, Id = id };
            dynamic result = _gateway.SelectSingle(query, parameters);
            return (result.count > 0);
        }
    }
}
