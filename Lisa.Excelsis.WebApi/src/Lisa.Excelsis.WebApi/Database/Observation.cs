using System.Collections.Generic;

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
                        observations.Add("(" + assessmentResult + ", " + category.Id + ", " + criterion.Order + @",  
                            '" + criterion.Title + "', '" + criterion.Description + "',  '" + criterion.Weight + "', '')");
                    }
                }

                var query = @"INSERT INTO Observations (AssessmentId, CategoryId, [Order], Title, Description, Weight, Result) VALUES ";
                query += string.Join(",", observations);
                _gateway.Insert(query, null);
            }
        }

        public bool ObservationExists(object assessmentId, object id)
        {
            var query = @"SELECT COUNT(*) as count FROM Observations
                          WHERE AssessmentId = @AssessmentId AND Id = @Id";
            dynamic result = _gateway.SelectSingle(query, new { AssessmentId = assessmentId, Id = id });
            return (result.count > 0);
        }
    }
}