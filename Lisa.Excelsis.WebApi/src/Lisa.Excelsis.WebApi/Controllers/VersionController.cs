using Microsoft.AspNet.Mvc;

namespace Lisa.Excelsis.WebApi.Controllers
{
    [Route("[controller]")]
    public class VersionController
    {
        [HttpGet]
        public IActionResult Get()
        {
            return new HttpOkObjectResult(new { Version = "1.0.0-Alpha-2" });
        }
    }
}