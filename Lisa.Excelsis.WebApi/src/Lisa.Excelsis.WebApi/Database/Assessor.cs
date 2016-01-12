using System.Collections.Generic;

namespace Lisa.Excelsis.WebApi
{
    partial class Database
    {
        public IEnumerable<object> FetchAssessors()
        {
            var query = @"SELECT Id, UserName, Firstname, Lastname
                          FROM Assessors";
            return _gateway.SelectMany(query);
        }
    }
}