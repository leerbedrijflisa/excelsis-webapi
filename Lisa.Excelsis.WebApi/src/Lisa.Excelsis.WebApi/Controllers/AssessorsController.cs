using Microsoft.AspNet.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Lisa.Excelsis.WebApi
{
    [Route("[controller]")]
    public class AssessorsController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            IEnumerable<object> result = _db.FetchAssessors();
            if (result.Count() == 0)
            {
                return new HttpNotFoundResult();
            }

            return new HttpOkObjectResult(result);
        }

        private readonly Database _db = new Database();
    }
}
