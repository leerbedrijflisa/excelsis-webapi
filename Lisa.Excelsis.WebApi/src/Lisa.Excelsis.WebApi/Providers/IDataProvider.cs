using System.Collections.Generic;

namespace Lisa.Excelsis.WebApi
{
    public interface IDataProvider
    {
        IEnumerable<IRowProvider> Rows { get; }
    }
}