using Microsoft.AspNet.Mvc;

namespace Lisa.Excelsis.WebApi
{
    public class UnprocessableEntityObjectResult : ObjectResult
    {
        public UnprocessableEntityObjectResult(object error) : base(error)
        {
            StatusCode = 422;
        }
    }
}
