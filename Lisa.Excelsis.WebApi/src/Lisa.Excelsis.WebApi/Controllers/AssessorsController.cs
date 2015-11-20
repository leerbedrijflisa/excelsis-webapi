using Microsoft.AspNet.Mvc;

namespace Lisa.Excelsis.WebApi
{
    [Route("[controller]")]
    public class AssessorsController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            var result = _db.FetchAssessors();
            return new HttpOkObjectResult(result);
        }

        private readonly Database _db = new Database();
    }
}
