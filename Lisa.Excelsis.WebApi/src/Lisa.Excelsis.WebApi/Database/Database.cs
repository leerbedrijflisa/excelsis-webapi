using System;

namespace Lisa.Excelsis.WebApi
{
    partial class Database : IDisposable
    {
        public void Dispose()
        {
            _gateway?.Dispose();
        }

        private Gateway _gateway = new Gateway(@"Data Source=(localdb)\v11.0;Initial Catalog=ExcelsisDb;Integrated Security=True");
    }
}
