using Microsoft.AspNet.Mvc;

namespace Lisa.Excelsis.WebApi
{
    [Route("[controller]")]
    public class CriteriaController : Controller
    {
        [HttpPost("{id}")]
        public IActionResult Post([FromBody] CriteriumPost criterium, int id)
        {
            if (id == 0 || !ModelState.IsValid || _db.AnyCriterium(id, criterium))
            {
                return new BadRequestResult();
            }

            _db.AddCriterium(id, criterium);
            var result = _db.FetchExam(id);

            return new CreatedResult("", result);
        }

        private readonly Database _db = new Database();
    }
}