using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Lisa.Excelsis.WebApi
{
    public static class ExtensionMethods
    {
        public static object Single(this SqlCommand command)
        {
            return command.Many().FirstOrDefault();
        }

        public static IEnumerable<object> Many(this SqlCommand command)
        {
            var reader = command.ExecuteReader();
            var dataProvider = new SqlDataProvider(reader);
            var mapper = new ObjectMapper();

            return mapper.Many(dataProvider);
        }

        public static object Insert(this SqlCommand command)
        {
            command.CommandText += "; SELECT SCOPE_IDENTITY()";
            return command.ExecuteScalar();
        }
    }
}
