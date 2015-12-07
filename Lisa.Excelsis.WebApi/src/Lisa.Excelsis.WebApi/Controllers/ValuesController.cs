using Microsoft.AspNet.Mvc;

namespace Lisa.Excelsis.WebApi.Controllers
{
    [Route("[controller]")]
    public class ValuesController : Controller
    {
        [HttpGet("marks")]
        public string GetMarks()
        {
            return "marks";
        }
    }
}
