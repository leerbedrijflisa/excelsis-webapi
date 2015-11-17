using Lisa.Common.Sql;
using System;
using System.Collections.Generic;

namespace Lisa.Excelsis.WebApi
{
    partial class Database : IDisposable
    {
        public IEnumerable<string> ErrorMessage { get; set; }

        public void Dispose()
        {
            _gateway?.Dispose();
        }
        
        private Gateway _gateway = new Gateway(@"Data Source=(localdb)\v11.0;Initial Catalog=ExcelsisDb;Integrated Security=True");
    }
}