using Microsoft.AspNet.Mvc;
using Newtonsoft.Json;
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
            IEnumerable<object> result = _db.FetchExams(filter);
            if (result.Count() == 0)
            {
                return new HttpNotFoundResult();
            }
            return new HttpOkObjectResult(result);
        }

        [HttpGet("{subject}/{cohort}")]
        public IActionResult Get([FromQuery] Filter filter, string subject, string cohort)
        {
            subject = _db.CleanParam(subject);

            IEnumerable<object> result = _db.FetchExams(filter, subject, cohort);
            if (result.Count() == 0)
            {
                return new HttpNotFoundResult();
            }

            return new HttpOkObjectResult(result);
        }

        [HttpGet("{subject}/{cohort}/{name}", Name = "exam")]
        public IActionResult Get(string subject, string cohort, string name)
        {
            subject = _db.CleanParam(subject);
            name = _db.CleanParam(name);

            var result = _db.FetchExam(subject, name, cohort);
            if(result == null)
            {
                return new HttpNotFoundResult();
            }

            return new HttpOkObjectResult(result);
        }

        [HttpPost]
        public IActionResult Post([FromBody] ExamPost exam)
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

                return new BadRequestObjectResult(errors);
            }

            if (exam == null)
            {
                return new BadRequestResult();
            }

            if (_db.ExamExists(exam))
            {
                errors.Add(new Error(1109, string.Format("The exam with subject '{0}', cohort '{1}', name '{2}' and crebo '{3}' already exists.", exam.Subject, exam.Cohort, exam.Name, exam.Crebo), new
                {
                    Subject = exam.Subject,
                    Cohort = exam.Cohort,
                    Name = exam.Name,
                    Crebo = exam.Crebo
                }));
            }


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
            string location = Url.RouteUrl("exam", new { subject = _db.CleanParam(exam.Subject), cohort = exam.Cohort, name = _db.CleanParam(exam.Name) }, Request.Scheme);
            return new CreatedResult(location, result);
        }

        private readonly Database _db = new Database();
    }
}