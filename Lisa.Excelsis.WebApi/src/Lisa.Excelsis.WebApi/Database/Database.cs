using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace Lisa.Excelsis.WebApi
{
    public class Database
    {
        public object SelectSingle(string query, object parameters = null)
        {
            var reader = Select(query, parameters);
            var dataProvider = new SqlRowProvider(reader);
            var mapper = new ObjectMapper();

            return mapper.Single(dataProvider);
        }

        public IEnumerable<object> SelectMany(string query, object parameters = null)
        {
            var reader = Select(query, parameters);
            var dataProvider = new SqlDataProvider(reader);
            var mapper = new ObjectMapper();

            return mapper.Many(dataProvider);            
        }
               
        private SqlDataReader Select(string query, object parameters)
        {
            using (var connection = new SqlConnection(@"Data Source=(localdb)\v11.0;Initial Catalog=ExcelsisDb;Integrated Security=True"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = query;
                command.CommandType = CommandType.Text;

                if (parameters != null)
                {
                    foreach (var parameter in parameters.GetType().GetProperties())
                    {
                        command.Parameters.Add(new SqlParameter(parameter.Name, parameter.GetValue(parameters)));
                    }
                }

               return command.ExecuteReader();
                
            }
        }
    }    
}
