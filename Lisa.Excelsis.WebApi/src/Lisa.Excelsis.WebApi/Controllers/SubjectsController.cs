using Microsoft.AspNet.Mvc;

namespace Lisa.Excelsis.WebApi
{
    [Route("[controller]")]
    public class SubjectsController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            var result = _db.FetchSubjects();
            return new HttpOkObjectResult(result);
        }

        private readonly Database _db = new Database();
    }
}