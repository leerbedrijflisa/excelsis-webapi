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
                        observations.Add("(" + criterion.Id + ", " + assessmentResult + ",'')");
                    }
                }

                var query = @"INSERT INTO Observations (Criterion_Id, Assessment_Id, Result) VALUES ";
                query += string.Join(",", observations);
                _gateway.Insert(query, null);
            }
        }
        private void PatchObservation(string action, int id, int observationId, string field, object value)
        {
            switch(action)
            {
                case "add":
                    if (field.ToLower() == "marks")
                    {
                        var query = @"INSERT INTO Marks ([Observation_Id], [Name]) 
                              VALUES (@Id, @Name)";
                        var parameters = new
                        {
                            Id = observationId,
                            Name = value
                        };
                        _gateway.Insert(query, parameters);
                    }
                    else
                    {
                        _errors.Add(new Error(1104, string.Format("The field '{0}' is not patchable.", field), new
                        {
                            FieldName = field
                        }));
                    }
                    break;
                case "replace":
                    if (field.ToLower() == "result")
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

                        _gateway.Update(query, parameters);
                    }
                    else
                    {
                        _errors.Add(new Error(1104, string.Format("The field '{0}' is not patchable.", field), new
                        {
                            FieldName = field
                        }));
                    }
                    break;
                case "remove":
                    if (field.ToLower() == "marks")
                    {
                        var query = @"DELETE FROM Marks
                                      WHERE Name = @Name";
                        var parameters = new
                        {
                            Name = value
                        };
                        _gateway.Update(query, parameters);
                    }
                    else
                    {
                        _errors.Add(new Error(1104, string.Format("The field '{0}' is not patchable.", field), new
                        {
                            FieldName = field
                        }));
                    }
                    break;
            }
        }
    }
}
