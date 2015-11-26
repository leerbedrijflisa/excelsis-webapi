using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lisa.Excelsis.WebApi
{
    [Route("[controller]")]
    public class ExamsController : Controller
    {
        [HttpGet]
        public IActionResult Get([FromQuery] Filter filter)
        {
            var result = _db.FetchExams(filter);
            return new HttpOkObjectResult(result);
        }

        [HttpGet("{subject}/{cohort}")]
        public IActionResult Get([FromQuery] Filter filter, string subject, string cohort)
        {
            subject = subject.Replace("-", " ");
            var result = _db.FetchExams(filter, subject, cohort);
            return new HttpOkObjectResult(result);
        }

        [HttpGet("{subject}/{cohort}/{name}", Name = "exam")]
        public IActionResult Get(string subject, string cohort, string name)
        {
            subject = Uri.UnescapeDataString(subject.Replace("-", " "));
            name = Uri.UnescapeDataString(name.Replace("-", " "));
            var result = _db.FetchExam(subject, name, cohort);
            return new HttpOkObjectResult(result);
        }

        [HttpPost]
        public IActionResult Post([FromBody] ExamPost exam)
        {
            List<Error> errors = new List<Error>();

            if (!ModelState.IsValid)
            {
                errors.Add(new Error(1110, "The json is invalid.", new { }));
                return new BadRequestObjectResult(errors);
            }
            _db.ExamExists(exam);

            errors.AddRange(_db.Errors);
            if (errors != null && errors.Any())
            {
                return new BadRequestObjectResult(errors);
            }

            var id = _db.AddExam(exam);

            errors.AddRange(_db.Errors);
            if (errors != null && errors.Any())
            {
                return new BadRequestObjectResult(errors);
            }

            var result = _db.FetchExam(id);
            string location = Url.RouteUrl("exam", new { subject = exam.Subject.Replace(" ", "-"), cohort = exam.Cohort, name = exam.Name.Replace(" ", "-") }, Request.Scheme);
            return new CreatedResult(location, result);
        }

        private readonly Database _db = new Database();
    }
}