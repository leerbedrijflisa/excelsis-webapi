using Microsoft.AspNet.Mvc;
using System;

namespace Lisa.Excelsis.WebApi
{
    [Route("[controller]")]
    public class ExamsController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            var result = _db.FetchExams();
            return new ObjectResult(result);
        }

        [HttpGet("{subject}/{cohort}")]
        public IActionResult Get(string subject, string cohort)
        {
            var result = _db.FetchExams(subject, cohort);
            return new ObjectResult(result);
        }

        [HttpGet("{subject}/{cohort}/{name}")]
        public IActionResult Get(string subject, string cohort, string name)
        {
            string examName = name.Replace("-", " ");
            var result = _db.FetchExam(subject, examName, cohort);
            return new ObjectResult(result);
        }

        [HttpPost]
        public IActionResult Post([FromBody] ExamPost exam)
        {
            if (!ModelState.IsValid || _db.ExamExists(exam))
            {
                return new BadRequestResult();
            }

            var id = _db.AddExam(exam);
            var result = _db.FetchExam(id);
            return new CreatedResult("", result);
        }

        private readonly Database _db = new Database();
    }
}