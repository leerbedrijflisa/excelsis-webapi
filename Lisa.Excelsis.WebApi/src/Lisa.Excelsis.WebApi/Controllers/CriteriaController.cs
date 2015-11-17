using Microsoft.AspNet.Mvc;

namespace Lisa.Excelsis.WebApi
{
    [Route("[controller]")]
    public class CriteriaController : Controller
    {
        [HttpPost("{id}")]
        public IActionResult Post([FromBody] CriterionPost criterion, int id)
        {
            if (!ModelState.IsValid || _db.CriterionExists(id, criterion))
            {
                return new BadRequestResult();
            }

            _db.AddCriterion(id, criterion);
            var result = _db.FetchExam(id);

            return new CreatedResult("", result);
        }

        private readonly Database _db = new Database();
    }
}