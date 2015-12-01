using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lisa.Excelsis.WebApi
{
    [Route("[controller]")]
    public class AssessmentsController : Controller
    {
        [HttpGet]
        public IActionResult Get([FromQuery] Filter filter)
        {
            IEnumerable<object> result = _db.FetchAssessments(filter);
            if (result.Count() == 0)
            {
                return new HttpNotFoundResult();
            }

            return new HttpOkObjectResult(result);
        }

        [HttpGet("{id}", Name = "assessment")]
        public IActionResult Get(int id)
        {
            var result = _db.FetchAssessment(id);
            if (result == null)
            {
                return new HttpNotFoundResult();
            }

            return new HttpOkObjectResult(result);
        }

        [HttpPatch("{id}")]
        public IActionResult Patch([FromBody] IEnumerable<Patch> patches, int id)
        {
            List<Error> errors = new List<Error>();

            if (!ModelState.IsValid)
            {
                var modelStateErrors = ModelState.Values.Where(E => E.Errors.Count > 0).SelectMany(E => E.Errors);
                foreach (var property in modelStateErrors)
                {
                    if (property.Exception == null)
                    {
                        errors.Add(new Error(1111, property.ErrorMessage, new { }));
                    }
                    else
                    {
                        return new BadRequestObjectResult(property.Exception.Message);
                    }
                }

                return new BadRequestObjectResult(errors);
            }

            if (patches == null)
            {
                errors.Add(new Error(1110, "The body is empty.", new { }));
                return new BadRequestObjectResult(errors);
            }

            _db.PatchAssessment(patches, id);

            errors.AddRange(_db.Errors);
            if (errors != null && errors.Any())
            {
                return new BadRequestObjectResult(errors);
            }

            var result = _db.FetchAssessment(id);
            return new HttpOkObjectResult(result);
        }


        [HttpPost("{subject}/{cohort}/{name}")]
        public IActionResult Post([FromBody] AssessmentPost assessment, string subject, string cohort, string name)
        {
            List<Error> errors = new List<Error>();

            if (!ModelState.IsValid)
            {
                var modelStateErrors = ModelState.Values.Where(E => E.Errors.Count > 0).SelectMany(E => E.Errors);
                foreach (var property in modelStateErrors)
                {
                    if (property.Exception == null)
                    {
                        errors.Add(new Error(1111, property.ErrorMessage, new { }));
                    }
                    else
                    {
                        return new BadRequestObjectResult(property.Exception.Message);
                    }
                }

                return new BadRequestObjectResult(errors);
            }

            if (assessment == null)
            {
                return new BadRequestResult();
            }

            subject = Uri.UnescapeDataString(subject.Replace("-", " "));
            name = Uri.UnescapeDataString(name.Replace("-", " "));

            var id = _db.AddAssessment(assessment, subject, name, cohort);

            errors.AddRange(_db.Errors);
            if (errors != null && errors.Any())
            {
                return new BadRequestObjectResult(errors);
            }

            var result = _db.FetchAssessment(id);
            string location = Url.RouteUrl("assessment", new { id = id }, Request.Scheme);
            return new CreatedResult(location, result);
        }

        private readonly Database _db = new Database();
    }
}