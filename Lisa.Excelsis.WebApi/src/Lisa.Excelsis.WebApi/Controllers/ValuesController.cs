using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;

namespace Lisa.Excelsis.WebApi.Controllers
{
    [Route("[controller]")]
    [Authorize(ActiveAuthenticationSchemes = "Bearer", Roles = "teacher")]
    public class ValuesController : Controller
    {
        [HttpGet("marks")]
        public IActionResult GetMarks()
        {
            string[] marks = new string[] { "maybe", "maybe_not", "skip", "unclear", "change" };
            return new HttpOkObjectResult(marks);
        }
    }
}