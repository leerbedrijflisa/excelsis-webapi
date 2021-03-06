﻿using Microsoft.AspNet.Mvc;
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
            return new HttpOkObjectResult(result);
        }

        private readonly Database _db = new Database();
    }
}
