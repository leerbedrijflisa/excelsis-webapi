using Lisa.Common.Sql;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.AspNet.Mvc.ModelBinding;
using System.Linq;
using Newtonsoft.Json;

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

        public string FatalError
        {
            get
            {
                return _fatalError;
            }
        }

        public object Execute(string query, object parameters)
        {
            return _gateway.SelectSingle(query, parameters);
        }

        public bool GetModelStateErrors(ModelStateDictionary ModelState)
        {
            bool fatalError = false;
            _errors = new List<Error>();
            var modelStateErrors = ModelState.Select(M => M).Where(X => X.Value.Errors.Count > 0);
            foreach (var property in modelStateErrors)
            {
                foreach (var error in property.Value.Errors)
                {
                    if (error.Exception == null)
                    {
                        _errors.Add(new Error(1101, new ErrorProps { Field = property.Key }));
                    }
                    else
                    {
                        if (Regex.IsMatch(error.Exception.Message, @"^Could not find member"))
                        {
                            _errors.Add(new Error(1103, new ErrorProps { Field = property.Key }));
                        }
                        else
                        {
                            fatalError = true;
                            _fatalError = JsonConvert.SerializeObject(error.Exception.Message);
                        }
                    }
                }
            }
            return (fatalError);
        }

        public void ClearErrors()
        {
            _errors.Clear();
        }

        public void ProcessTransactions(IEnumerable<QueryData> transactions)
        {
            _gateway.ProcessTransaction(() =>
            {
                foreach(QueryData transaction in transactions)
                {
                    Execute(transaction.Query, transaction.Parameters);
                }                
            });
        }

        public static List<Error> _errors { get; set; }

        private static Action<string, object> Apply(Action<string, object> transaction)
        {
            return transaction;
        }

        private string _fatalError { get; set; }

        private Gateway _gateway = new Gateway(Environment.GetEnvironmentVariable("ConnectionString"));
    }
}