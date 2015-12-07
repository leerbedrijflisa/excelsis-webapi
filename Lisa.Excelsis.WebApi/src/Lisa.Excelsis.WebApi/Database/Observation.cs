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

                var query = @"INSERT INTO Observations (Criterion_Id, Assessment_Id, Result) VALUES ";
                query += string.Join(",", observations);
                _gateway.Insert(query, null);
            }
        }

        public void AddMark(Patch patch)
        {
            if (Regex.IsMatch(patch.Value.ToString().ToLower(), @"^[a-zA-Z]*$"))
            {
                var field = patch.Field.Split('/');
                var query = @"INSERT INTO Marks ([Observation_Id], [Name]) 
                    VALUES (@Id, @Name)";
                var parameters = new
                {
                    Id = field[1],
                    Name = patch.Value
                };
                _gateway.Insert(query, parameters);
            }
            else
            {
                _errors.Add(new Error(0, string.Format("The value '{0}' is not a word.", patch.Value), new
                {
                    Value = patch.Value
                }));
            }
        }

        public void RemoveMark(Patch patch)
        {
            if (Regex.IsMatch(patch.Value.ToString().ToLower(), @"^[a-zA-Z]*$"))
            {
                var query = @"DELETE FROM Marks
                              WHERE Name = @Name";
                var parameters = new
                {
                    Name = patch.Value
                };
                _gateway.Update(query, parameters);
            }
            else
            {
                _errors.Add(new Error(0, string.Format("The value '{0}' is not a word.", patch.Value), new
                {
                    Value = patch.Value
                }));
            }
        }

        public void ReplaceResult(int id, Patch patch)
        {
            if (Regex.IsMatch(patch.Value.ToString().ToLower(), @"^seen$|^unseen$"))
            {
                var field = patch.Field.Split('/');
                var query = @"UPDATE Observations
                              SET result = @Value
                              WHERE Assessment_Id = @Id AND Id = @ObservationId";
                var parameters = new
                {
                    value = patch.Value,
                    Id = id,
                    ObservationId = field[1]
                };
                _gateway.Update(query, parameters);
            }
            else
            {
                _errors.Add(new Error(0, string.Format("The value '{0}' has to be 'seen' or 'unseen'.", patch.Value), new
                {
                    Value = patch.Value
                }));
            }
        }

        private bool ObservationExists(int id)
        {
            var query = @"SELECT COUNT(*) as count FROM Observations
                          WHERE Id = @id";
            var parameters = new
            {
                Id = id
            };
            dynamic result = _gateway.SelectSingle(query, parameters);

            return (result.count > 0);
        }
    }
}
