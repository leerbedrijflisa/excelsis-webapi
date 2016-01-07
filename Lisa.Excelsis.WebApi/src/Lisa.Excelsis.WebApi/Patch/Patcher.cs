using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Lisa.Excelsis.WebApi
{
    public class Patcher
    {
        protected void Build(string action, int id, Patch patch, string pattern, Func<int, Patch, QueryData> func)
        {
            if (Regex.IsMatch(patch.Field.ToLower(), pattern))
            {
                if (patch.Action.ToLower() == action)
                {
                    QueryData transaction = func(id, patch);
                    transactions.Add(transaction);
                }
            }
        }

        protected void ExecuteTransactions()
        {

        }

        protected List<QueryData> transactions = new List<QueryData>();
    }
}
