using Microsoft.AspNet.Mvc;
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
            return new HttpOkObjectResult(result);
        }

        [HttpGet("{subject}/{cohort}")]
        public IActionResult Get([FromQuery] Filter filter, string subject, string cohort)
        {
            subject = _db.CleanParam(subject);

            IEnumerable<object> result = _db.FetchExams(filter, subject, cohort);
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
        [HttpPatch("{id}")]
        public IActionResult Patch([FromBody] List<Patch> patches, int id)
        {
            if (!ModelState.IsValid)
            {
                if (_db.GetModelStateErrors(ModelState))
                {
                    return new BadRequestObjectResult(_db.FatalError);
                }
                else
                {
                    return new UnprocessableEntityObjectResult(_db.Errors);
                }
            }

            if (patches == null)
            {
                return new UnprocessableEntityObjectResult(new Error(1100));
            }

            if (!_db.ExamExists(id))
            {
                return new HttpNotFoundResult();
            }

            _db.PatchExam(patches, id);

            if (_db.Errors.Any())
            {
                return new BadRequestObjectResult(_db.Errors);
            }

            var result = _db.FetchExam(id);
            return new HttpOkObjectResult(result);
        }

        [HttpPost]
        public IActionResult Post([FromBody] ExamPost exam)
        {
            List<Error> errors = new List<Error>();

            if (!ModelState.IsValid)
            {
                if (_db.GetModelStateErrors(ModelState))
                {
                    return new BadRequestObjectResult(_db.FatalError);
                }
                else
                {
                    return new UnprocessableEntityObjectResult(_db.Errors);
                }
            }

            if (exam == null)
            {
                return new UnprocessableEntityObjectResult(new Error(1100));
            }

            if (_db.ExamExists(exam))
            {
                errors.Add(new Error(1301, new { subject = exam.Subject, cohort = exam.Cohort, name = exam.Name, crebo = exam.Crebo }));
            }

            if (errors.Any())
            {
                return new UnprocessableEntityObjectResult(errors);
            }

            var id = _db.AddExam(exam);
            if (_db.Errors.Any())
            {
                return new UnprocessableEntityObjectResult(_db.Errors);
            }

            var result = _db.FetchExam(id);
            string location = Url.RouteUrl("exam", new { subject = _db.CleanParam(exam.Subject), cohort = exam.Cohort, name = _db.CleanParam(exam.Name) }, Request.Scheme);
            return new CreatedResult(location, result);
        }

        private readonly Database _db = new Database();
    }
}