using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Lisa.Excelsis.WebApi
{
    partial class Database
    {
        public bool QueryBuilderReplace(dynamic resource, JToken value, PatchPropInfo parameters)
        {
            var query = @"UPDATE " + parameters.Child + @" 
                          SET [" + parameters.Property + @"] = @Value
                          WHERE Id = @Id";
            _gateway.Update(query, new { Id = parameters.ChildId, Value = value.ToString() });
            return true;
        }

        public bool QueryBuilderRemove(dynamic resource, JToken value, PatchPropInfo parameters)
        {
            var query = @"DELETE FROM " + parameters.Child + @"  WHERE Id = @Id";
           _gateway.Update(query, new { Id = value.ToString()});
            return true;
        }

        public bool QueryBuilderMove(dynamic resource, JToken value, PatchPropInfo parameters)
        {
            var query = @"UPDATE " + parameters.Child + @" 
                          SET " + parameters.Target + @" = @TargetId 
                          WHERE Id = @Id";
            var target = value.ToString().Split('/');
            _gateway.Update(query, new { Id = parameters.ChildId, TargetId = target[1] });
            return true;
        }
        public bool QueryBuilderAdd(dynamic resource, JToken value, PatchPropInfo parameters)
        {
            List<string> propParams = new List<string>();
            List<string> valueParams = new List<string>();

            var query = @"INSERT INTO " + parameters.Child;
            foreach (var prop in value as JObject)
            {
                propParams.Add("[" + prop.Key + "]");
                valueParams.Add("'" + prop.Value.ToString() + "'");
            }

            if (parameters.Parent != string.Empty && parameters.ParentId != string.Empty)
            {
                propParams.Add(parameters.Parent.ToString());
                valueParams.Add(parameters.ParentId.ToString());
            }

            propParams.Add(resource.Name.ToString());
            valueParams.Add(resource.Value.ToString());

            string props = string.Join(",", propParams);
            string values = string.Join(",", valueParams);

            query += "(" + props + ")" + " VALUES (" + values + ")";
            _gateway.Insert(query, new { });

            return true;
        }

        public bool QueryBuilderRemoveMark(dynamic resource, JToken value, PatchPropInfo parameters)
        {
            var query = @"DELETE FROM " + parameters.Child + @"  WHERE Name = @Id";
            _gateway.Update(query, new { Id = value.ToString() });
            return true;
        }

        public bool QueryBuilderAddMark(dynamic resource, JToken value, PatchPropInfo parameters)
        {
            List<string> propParams = new List<string>();
            List<string> valueParams = new List<string>();

            var query = @"INSERT INTO " + parameters.Child;
           
            propParams.Add("[name]");
            valueParams.Add("'" + value.ToString() + "'");

            if (parameters.Parent != string.Empty && parameters.ParentId != string.Empty)
            {
                propParams.Add(parameters.Parent.ToString());
                valueParams.Add(parameters.ParentId.ToString());
            }

            propParams.Add(resource.Name.ToString());
            valueParams.Add(resource.Value.ToString());

            string props = string.Join(",", propParams);
            string values = string.Join(",", valueParams);

            query += "(" + props + ")" + " VALUES (" + values + ")";
            _gateway.Insert(query, new { });

            return true;
        }
    }
}
