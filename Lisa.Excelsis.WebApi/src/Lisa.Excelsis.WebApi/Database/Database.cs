using Lisa.Common.Sql;
using System;
using System.Collections.Generic;

namespace Lisa.Excelsis.WebApi
{
    partial class Database : IDisposable
    {
        public void Dispose()
        {
            _gateway?.Dispose();
        }

        public IEnumerable<Error> Errors
        {
            get
            {
                return _errors;
            }
        }

        private List<Error> _errors { get; set; }

        private Gateway _gateway = new Gateway(Environment.GetEnvironmentVariable("ConnectionString"));
    }
}