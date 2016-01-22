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
            subject = Utils.CleanParam(subject);

            IEnumerable<object> result = _db.FetchExams(filter, subject, cohort);
            return new HttpOkObjectResult(result);
        }

        [HttpGet("{subject}/{cohort}/{name}", Name = "exam")]
        public IActionResult Get(string subject, string cohort, string name)
        {
            subject = Utils.CleanParam(subject);
            name = Utils.CleanParam(name);

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
            ExamValidator validator = new ExamValidator();
            List<Error> errors = new List<Error>();

            if (!_db.ExamExists(id))
            {
                return new HttpNotFoundResult();
            }

            if (!_db.IsModelStateValid(ModelState, patches))
            {
                return _db.ModelStateErrors;
            }

            errors.AddRange(validator.ValidatePatches(id, patches));
            if (errors.Any())
            {
                return new UnprocessableEntityObjectResult(errors);
            }

            _db.PatchExam(patches, id);

            var result = _db.FetchExam(id);
            return new HttpOkObjectResult(result);
        }

        [HttpPost]
        public IActionResult Post([FromBody] ExamPost exam)
        {
            ExamValidator validator = new ExamValidator();
            List<Error> errors = new List<Error>();

            if (!_db.IsModelStateValid(ModelState, exam))
            {
                return _db.ModelStateErrors;
            }

            if (_db.ExamExists(exam))
            {
                return new UnprocessableEntityObjectResult(new Error(1301, new ErrorProps { Subject = exam.Subject, Cohort = exam.Cohort, Name = exam.Name, Crebo = exam.Crebo }));
            }

            errors.AddRange(validator.ValidatePost(exam));
            if (errors.Any())
            {
                return new UnprocessableEntityObjectResult(errors);
            }

            var id = _db.AddExam(exam);
            var result = _db.FetchExam(id);
            string location = Url.RouteUrl("exam", new { subject = Utils.CleanParam(exam.Subject), cohort = exam.Cohort, name = Utils.CleanParam(exam.Name) }, Request.Scheme);
            return new CreatedResult(location, result);
        }

        private readonly Database _db = new Database();
    }
}