﻿using System.Collections.Generic;

namespace Lisa.Excelsis.WebApi
{
    partial class Database
    {
        public IEnumerable<object> FetchSubjects()
        {
            var query = @"SELECT DISTINCT subject as Name
                          FROM Exams";
            return _gateway.SelectMany(query);
        }
    }
}