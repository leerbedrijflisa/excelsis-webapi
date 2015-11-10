using System;
using System.Reflection;
using System.Data;
using System.Data.SqlClient;

namespace Lisa.Excelsis.WebApi
{
    public class Gateway : IDisposable
    {
        public Gateway(string connectionString)
        {
            _connection = new SqlConnection(connectionString);
            _connection.Open();
        }

        public SqlCommand Execute(string query, object parameters = null)
        {
            var command = _connection.CreateCommand();
            command.CommandText = query;
            command.CommandType = CommandType.Text;

            if (parameters != null)
            {
                foreach (var parameter in parameters.GetType().GetProperties())
                {
                    command.Parameters.Add(new SqlParameter(parameter.Name, parameter.GetValue(parameters)));
                }
            }
            return command;
        }

        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
        }

        private SqlConnection _connection;
    }
}
