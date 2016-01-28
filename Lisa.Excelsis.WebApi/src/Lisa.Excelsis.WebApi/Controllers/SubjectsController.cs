using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using System.Collections.Generic;

namespace Lisa.Excelsis.WebApi
{
    [Route("[controller]")]
    [Authorize(ActiveAuthenticationSchemes = "Bearer", Roles = "teacher")]
    public class SubjectsController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            IEnumerable<object> result = _db.FetchSubjects();
            return new HttpOkObjectResult(result);
        }

        private readonly Database _db = new Database();
    }
}