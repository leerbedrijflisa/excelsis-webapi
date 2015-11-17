using Microsoft.AspNet.Mvc;
using System;

namespace Lisa.Excelsis.WebApi
{
    [Route("[controller]")]
    public class AssessmentsController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            var result = _db.FetchAssessments();
            return new ObjectResult(result);
        }
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var result = _db.FetchAssessment(id);
            return new ObjectResult(result);
        }

        [HttpPost("{subject}/{name}/{cohort}")]
        public IActionResult Post([FromBody] AssessmentPost assessment, string subject, string name, string cohort)
        {
            string examName = name.Replace("-", " ");
            if (!ModelState.IsValid || subject == null || examName == null || cohort == null)
            {
                return new BadRequestResult();
            }

            var id = _db.AddAssessment(assessment, subject, examName, cohort);

            if (id == null)
            {
                return new BadRequestResult();
            }

            var result = _db.FetchAssessment(id);

            return new CreatedResult("", result);
        }

        private readonly Database _db = new Database();
    }
}