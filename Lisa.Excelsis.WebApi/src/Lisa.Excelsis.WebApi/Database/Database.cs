using Lisa.Common.Sql;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.AspNet.Mvc.ModelBinding;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.AspNet.Mvc;

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

        public IActionResult ModelStateErrors { get; private set; }

        public bool IsModelStateValid(ModelStateDictionary ModelState, dynamic model)
        {
            _errors = new List<Error>();

            if (model == null)
            {               
                ModelStateErrors = new BadRequestResult();
                return false;
            }

            if (!ModelState.IsValid)
            {              
                  
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
                                ModelStateErrors = new BadRequestObjectResult(JsonConvert.SerializeObject(error.Exception.Message));
                                return false;
                            }
                        }
                    }
                }

                ModelStateErrors = new UnprocessableEntityObjectResult(_errors);
                return false;
            }
            return true;
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

        public void ClearErrors()
        {
            _errors.Clear();
        }
       
        private string _fatalError { get; set; }

        private Gateway _gateway = new Gateway(Environment.GetEnvironmentVariable("ConnectionString"));
    }
}