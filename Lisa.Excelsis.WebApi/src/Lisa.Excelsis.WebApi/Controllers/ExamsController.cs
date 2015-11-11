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

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var result = _db.FetchExam(id);
            return new ObjectResult(result);
        }

        [HttpPost]
        public IActionResult Post([FromBody] ExamPost exam)
        {
            if (!ModelState.IsValid || _db.AnyExam(exam))
            {
                return new BadRequestResult();
            }

            var id = _db.AddExam(exam);
            var result = _db.FetchExam(Convert.ToInt32(id));

            return new CreatedResult("", result);
        }

        [HttpPost("{id}/criterium")]
        public IActionResult Post([FromBody] CriteriumPost criterium, int id)
        {
            if (id == 0 || !ModelState.IsValid || _db.AnyCriterium(id, criterium))
            {
                return new BadRequestResult();
            }

            _db.AddCriterium(id, criterium);
            var result = _db.FetchExam(Convert.ToInt32(id));

            return new CreatedResult("", result);
        }


        private readonly Database _db = new Database();
    }
}
