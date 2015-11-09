using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lisa.Excelsis.WebApi
{
    public interface IRowProvider
    {
        IEnumerable<KeyValuePair<string, object>> Fields { get; }
    }
}