using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace Lisa.Excelsis.WebApi
{
    public sealed class Gateway : IDisposable
    {
        public Gateway(string connectionString)
        {
            _connection = new SqlConnection(connectionString);
            _connection.Open();
        }

        public object SelectSingle(string query, object parameters = null)
        {
            return SelectMany(query, parameters).FirstOrDefault();
        }

        public IEnumerable<object> SelectMany(string query, object parameters = null)
        {
            var command = CreateCommand(query, parameters);
            using (var reader = command.ExecuteReader())
            {
                var dataProvider = new SqlDataProvider(reader);
                var mapper = new ObjectMapper();
                return mapper.Many(dataProvider);
            }
        }

        public object Insert(string query, object parameters)
        {
            var command = CreateCommand(query, parameters);
            command.ExecuteNonQuery();

            command = CreateCommand("select @@identity");
            return command.ExecuteScalar();
        }

        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
        }

        private SqlCommand CreateCommand(string query, object parameters = null)
        {
            var command = _connection.CreateCommand();
            command.CommandText = query;
            command.CommandType = CommandType.Text;

            if (parameters != null)
            {
                foreach (var property in parameters.GetType().GetProperties())
                {
                    var parameter = new SqlParameter(property.Name, property.GetValue(parameters));
                    command.Parameters.Add(parameter);
                }
            }

            return command;
        }

        private SqlConnection _connection;
    }
}