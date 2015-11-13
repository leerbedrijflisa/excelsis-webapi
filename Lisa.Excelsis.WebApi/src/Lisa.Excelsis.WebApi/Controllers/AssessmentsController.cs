using Microsoft.AspNet.Mvc;
using System;

namespace Lisa.Excelsis.WebApi
{
    [Route("[controller]")]
    public class AssessmentsController : Controller
    {
        [HttpPost("{subject}/{name}/{cohort}")]
        public IActionResult Post([FromBody]AssessmentPost assessment, string subject, string name, string cohort)
        {
            if (!ModelState.IsValid && subject != null && name != null && cohort != null)
            {
                return new BadRequestResult();
            }

            var id = _db.AddAssessment(assessment, subject, name, cohort);
            var result = _db.FetchAssessment(Convert.ToInt32(id));

            return new CreatedResult("", result);
        }

        private readonly Database _db = new Database();
    }
}
