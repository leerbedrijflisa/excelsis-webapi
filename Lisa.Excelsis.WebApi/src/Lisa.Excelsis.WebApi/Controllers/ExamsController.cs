using Microsoft.AspNet.Mvc;

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
            var result = _db.FetchExams(filter, subject, cohort);
            return new HttpOkObjectResult(result);
        }

        [HttpGet("{subject}/{cohort}/{name}", Name = "exam")]
        public IActionResult Get(string subject, string cohort, string name)
        {
            string examName = name.Replace("-", " ");
            var result = _db.FetchExam(subject, examName, cohort);
            return new HttpOkObjectResult(result);
        }

        [HttpPost]
        public IActionResult Post([FromBody] ExamPost exam)
        {
            if (!ModelState.IsValid || _db.ExamExists(exam))
            {
                return new BadRequestObjectResult(new { errorMessage = "Invalid json or exam already exists." });
            }

            var id = _db.AddExam(exam);
            var result = _db.FetchExam(id);
            string location = Url.RouteUrl("exam", new { subject = exam.Subject, cohort = exam.Cohort, name = exam.Name }, Request.Scheme);
            return new CreatedResult(location, result);
        }

        private readonly Database _db = new Database();
    }
}