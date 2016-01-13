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
            ExamValidator validator = new ExamValidator();

            if (!_db.ExamExists(id))
            {
                return new HttpNotFoundResult();
            }

            if (!_db.IsModelStateValid(ModelState, patches))
            {
                return _db.ModelStateErrors;
            }

            validator.ValidatePatches(id, patches);
            if (!validator.IsPatchValid())
            {
                return validator.PatchErrors;
            }

            _db.PatchExam(patches, id);

            var result = _db.FetchExam(id);
            return new HttpOkObjectResult(result);
        }

        [HttpPost]
        public IActionResult Post([FromBody] ExamPost exam)
        {
            if (_db.IsModelStateValid(ModelState, exam))
            {
                return _db.ModelStateErrors;
            }

            if (_db.ExamExists(exam))
            {
                return new UnprocessableEntityObjectResult(new Error(1301, new ErrorProps { Subject = exam.Subject, Cohort = exam.Cohort, Name = exam.Name, Crebo = exam.Crebo }));
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
    }
}