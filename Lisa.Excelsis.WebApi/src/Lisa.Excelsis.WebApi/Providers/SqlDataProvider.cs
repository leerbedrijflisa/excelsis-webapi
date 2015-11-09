using System.Collections.Generic;
using System.Data.SqlClient;

namespace Lisa.Excelsis.WebApi
{
    public class SqlDataProvider : IDataProvider
    {
        public SqlDataProvider(SqlDataReader reader)
        {
            _reader = reader;
        }

        public IEnumerable<IRowProvider> Rows
        {
            get
            {
                while (_reader.Read())
                {
                    yield return new SqlRowProvider(_reader);
                }
            }
        }

        private SqlDataReader _reader;
    }
}