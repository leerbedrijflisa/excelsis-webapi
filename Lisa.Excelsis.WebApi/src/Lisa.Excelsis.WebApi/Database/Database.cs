﻿using Lisa.Common.Sql;
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
    }
}