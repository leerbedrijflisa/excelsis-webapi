using Microsoft.AspNet.Mvc;
using System.Collections.Generic;
using System.Linq;

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

        [HttpPatch("{id}")]
        public IActionResult Patch([FromBody] IEnumerable<Patch> patches, int id)
        {
            _db.PatchAssessment(patches, id);
            if (_db.ErrorMessages != null && _db.ErrorMessages.Any())
            {
                return new BadRequestObjectResult(_db.ErrorMessages);
            }
            //return new HttpOkObjectResult(result);
            return new HttpOkResult();
        }


        [HttpPost("{subject}/{cohort}/{name}")]
        public IActionResult Post([FromBody] AssessmentPost assessment, string subject, string cohort, string name)
        {
            string examName = name.Replace("-", " ");
            if (!ModelState.IsValid || subject == null || examName == null || cohort == null)
            {
                return new BadRequestObjectResult(new { errorMessage = "Invalid json or url." });
            }

            var id = _db.AddAssessment(assessment, subject, examName, cohort);

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