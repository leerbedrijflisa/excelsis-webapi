using Microsoft.AspNet.Mvc;
using System;

namespace Lisa.Excelsis.WebApi
{
    [Route("[controller]")]
    public class AssessmentsController : Controller
    {
        [HttpGet]
        public IActionResult Get([FromQuery] Filter filter)
        {
            var result = _db.FetchAssessments(filter);
            return new HttpOkObjectResult(result);
        }

        [HttpGet("{id}", Name = "assessment")]
        public IActionResult Get(int id)
        {
            var result = _db.FetchAssessment(id);
            return new HttpOkObjectResult(result);
        }

        [HttpPost("{subject}/{cohort}/{name}")]
        public IActionResult Post([FromBody] AssessmentPost assessment, string subject, string cohort, string name)
        {
            subject = Uri.UnescapeDataString(subject.Replace("-", " "));
            name = Uri.UnescapeDataString(name.Replace("-", " "));
            if (!ModelState.IsValid || subject == null || name == null || cohort == null)
            {
                return new BadRequestObjectResult(new { errorMessage = "Invalid json or url." });
            }

            var id = _db.AddAssessment(assessment, subject, name, cohort);

            if (id == null)
            {
                return new BadRequestObjectResult(_db.ErrorMessages);
            }

            var result = _db.FetchAssessment(id);
            string location = Url.RouteUrl("assessment", new { id = id }, Request.Scheme);
            return new CreatedResult(location, result);
        }

        private readonly Database _db = new Database();
    }
}