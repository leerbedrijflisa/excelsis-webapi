using Microsoft.AspNet.Mvc;
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
                errors.Add(new Error(1110, "The json is invalid.", new { }));
                return new BadRequestObjectResult(errors);
            }

            _db.AddCriterion(id, criterion);

            errors.AddRange(_db.Errors);
            if (errors != null && errors.Any())
            {
                return new BadRequestObjectResult(errors);
            }

            dynamic result = _db.FetchExam(id);
            string location = Url.RouteUrl("exam", new { subject = result.Subject, cohort = result.Cohort, name = result.Name }, Request.Scheme);
            return new CreatedResult(location, result);
        }

        private readonly Database _db = new Database();
    }
}