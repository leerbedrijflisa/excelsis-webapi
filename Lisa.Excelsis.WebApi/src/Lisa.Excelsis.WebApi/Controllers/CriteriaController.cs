using Microsoft.AspNet.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Lisa.Excelsis.WebApi
{
    [Route("[controller]")]
    public class CriteriaController : Controller
    {
        [HttpPost("{id}")]
        public IActionResult Post([FromBody] CriterionPost criterion, int id)
        {
            List<Error> errors = new List<Error>();

            if (!ModelState.IsValid)
            {
                var modelStateErrors = ModelState.Select(M => M).Where(X => X.Value.Errors.Count > 0);
                foreach (var property in modelStateErrors)
                {
                    var propertyName = property.Key;
                    foreach (var error in property.Value.Errors)
                    {
                        if (error.Exception == null)
                        {
                            errors.Add(new Error(1111, error.ErrorMessage, new { field = propertyName }));
                        }
                        else
                        {
                            return new BadRequestObjectResult(JsonConvert.SerializeObject(error.Exception.Message));
                        }
                    }
                }
            }
            
            if (criterion == null)
            {
                errors.Add(new Error(1110, "The body is empty.", new { }));
                return new BadRequestObjectResult(errors);
            }

            _db.AddCriterion(id, criterion);

            errors.AddRange(_db.Errors);
            if (errors != null && errors.Any())
            {
                return new BadRequestObjectResult(errors);
            }

            dynamic result = _db.FetchExam(id);
            string location = Url.RouteUrl("exam", new { subject = _db.CleanParam(result.Subject), cohort = result.Cohort, name = _db.CleanParam(result.Subject) }, Request.Scheme);
            return new CreatedResult(location, result);
        }

        private readonly Database _db = new Database();
    }
}