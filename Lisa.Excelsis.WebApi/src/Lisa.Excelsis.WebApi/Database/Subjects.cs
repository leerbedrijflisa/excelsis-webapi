using System.Collections.Generic;

namespace Lisa.Excelsis.WebApi
{
    partial class Database
    {
        public IEnumerable<object> FetchSubjects()
        {
            var query = @"SELECT subject FROM Exams GROUP BY subject";
            return _gateway.SelectMany(query);
        }
    }
}