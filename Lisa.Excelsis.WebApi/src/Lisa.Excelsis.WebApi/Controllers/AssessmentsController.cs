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
            IEnumerable<object> result = _db.FetchAssessments(filter);
            if (result.Count() == 0)
            {
                return new HttpNotFoundResult();
            }

            return new HttpOkObjectResult(result);
        }

        [HttpGet("{id}", Name = "assessment")]
        public IActionResult Get(int id)
        {
            var result = _db.FetchAssessment(id);
            if (result == null)
            {
                return new HttpNotFoundResult();
            }

            return new HttpOkObjectResult(result);
        }

        [HttpPatch("{id}")]
        public IActionResult Patch([FromBody] IEnumerable<Patch> patches, int id)
        {
            List<Error> errors = new List<Error>();
            if (!ModelState.IsValid || patches == null)
            {
                errors.Add(new Error(1110, "The json is malformed.", new { }));
                return new BadRequestObjectResult(errors);
            }


            _db.PatchAssessment(patches, id);

            errors.AddRange(_db.Errors);
            if (errors != null && errors.Any())
            {
                return new BadRequestObjectResult(errors);
            }
            //return new HttpOkObjectResult(result);
            return new HttpOkResult();
        }


        [HttpPost("{subject}/{cohort}/{name}")]
        public IActionResult Post([FromBody] AssessmentPost assessment, string subject, string cohort, string name)
        {
            List<Error> errors = new List<Error>();

            string examName = name.Replace("-", " ");
            if (!ModelState.IsValid || assessment == null)
            {
                errors.Add(new Error(1110, "The json is malformed.", new { }));
                return new BadRequestObjectResult(errors);
            }

            var id = _db.AddAssessment(assessment, subject, examName, cohort);

            errors.AddRange(_db.Errors);
            if (errors != null && errors.Any())
            {
                return new BadRequestObjectResult(errors);
            }

            var result = _db.FetchAssessment(id);
            string location = Url.RouteUrl("assessment", new { id = id }, Request.Scheme);
            return new CreatedResult(location, result);
        }

        private readonly Database _db = new Database();
    }
}