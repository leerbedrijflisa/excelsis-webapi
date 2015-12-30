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
            subject = Misc.CleanParam(subject);

            IEnumerable<object> result = _db.FetchExams(filter, subject, cohort);
            return new HttpOkObjectResult(result);
        }

        [HttpGet("{subject}/{cohort}/{name}", Name = "exam")]
        public IActionResult Get(string subject, string cohort, string name)
        {
            subject = Misc.CleanParam(subject);
            name = Misc.CleanParam(name);

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

            if (patches == null)
            {
                return new UnprocessableEntityObjectResult(new Error(1100));
            }

            if (!_db.ExamExists(id))
            {
                return new HttpNotFoundResult();
            }

            _db.PatchExam(patches, id);

            if (_db.Errors.Any() || _val.Errors.Any())
            {
                errors.AddRange(_db.Errors);
                errors.AddRange(_val.Errors);
                _val.ClearErrors();
                _db.ClearErrors();
                return new UnprocessableEntityObjectResult(errors);
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
                errors.Add(new Error(1301, new ErrorProps { Subject = exam.Subject, Cohort = exam.Cohort, Name = exam.Name, Crebo = exam.Crebo }));
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
            string location = Url.RouteUrl("exam", new { subject = Misc.CleanParam(exam.Subject), cohort = exam.Cohort, name = Misc.CleanParam(exam.Name) }, Request.Scheme);
            return new CreatedResult(location, result);
        }

        private readonly Database _db = new Database();
        private readonly Validate _val = new Validate();
    }
}