using Lisa.Common.Sql;
using System;
using System.Collections.Generic;

namespace Lisa.Excelsis.WebApi
{
    partial class Database : IDisposable
    {
        public IEnumerable<string> ErrorMessages
        {
            get
            {
                return _errorMessages;
            }
        }

        public void Dispose()
        {
            _gateway?.Dispose();
        }

        private List<string> _errorMessages { get; set; }

        private Gateway _gateway = new Gateway(@"Data Source=(localdb)\v11.0;Initial Catalog=ExcelsisDb;Integrated Security=True");
        //private Gateway _gateway = new Gateway(@"Server=tcp:leerbedrijflisa.database.windows.net,1433;Database=excelsis-develop-db;User ID=leerbedrijflisa@leerbedrijflisa;Password=j1*K36SF$PF#pG#M0M34ldb%zCWFYAP!F*%lNlp%Fc1o5VvyBP^YLj^hO;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
    }
}