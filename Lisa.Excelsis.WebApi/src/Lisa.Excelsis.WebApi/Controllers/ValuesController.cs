﻿using Microsoft.AspNet.Mvc;

namespace Lisa.Excelsis.WebApi.Controllers
{
    [Route("[controller]")]
    public class ValuesController : Controller
    {
        [HttpGet("marks")]
        public IActionResult GetMarks()
        {
            string[] marks = new string[] { "maybe", "maybe not", "skip", "unclear", "change" };
            return new HttpOkObjectResult(marks);
        }
    }
}
