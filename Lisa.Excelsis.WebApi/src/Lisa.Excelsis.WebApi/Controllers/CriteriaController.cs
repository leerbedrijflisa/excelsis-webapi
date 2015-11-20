using Microsoft.AspNet.Mvc;

namespace Lisa.Excelsis.WebApi
{
    [Route("[controller]")]
    public class CriteriaController : Controller
    {
        [HttpPost("{id}")]
        public IActionResult Post([FromBody] CriterionPost criterion, int id)
        {
            if (!ModelState.IsValid || _db.CriterionExists(id, criterion))
            {
                return new BadRequestObjectResult(new { errorMessage = "Invalid json or url." });
            }

            _db.AddCriterion(id, criterion);
            dynamic result = _db.FetchExam(id);
            string location = Url.RouteUrl("exam", new { subject = result.Subject, cohort = result.Cohort, name = result.Name }, Request.Scheme);
            return new CreatedResult(location, result);
        }

        private readonly Database _db = new Database();
    }
}