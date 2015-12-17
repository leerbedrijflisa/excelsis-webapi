using Newtonsoft.Json.Linq;

namespace Lisa.Excelsis.WebApi
{
    partial class Database
    {
        public bool QueryBuilderReplace(dynamic parameters, JToken value)
        {
            var query = @"UPDATE " + parameters.Child + @" 
                          SET " + parameters.Property + @" = @Value
                          WHERE Id = @Id";
            _gateway.Update(query, new { Id = parameters.ChildId, Value = value });
            return true;
        }

        public bool QueryBuilderRemove(dynamic parameters, JToken value)
        {
            var query = @"DELETE FROM " + parameters.Child + @"  WHERE Id = @Id";
           _gateway.Update(query, new { Id = value});
            return true;
        }

        public bool QueryBuilderMove(dynamic parameters, JToken value)
        {
            var query = @"UPDATE " + parameters.Child + @" 
                          SET " + parameters.Target + @" = @TargetId 
                          WHERE Id = @Id";
            var target = value.ToString().Split('/');
            _gateway.Update(query, new { Id = parameters.ChildId, TargetId = target[1] });
            return true;
        }
    }
}
