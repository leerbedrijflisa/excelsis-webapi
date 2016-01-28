using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Authorization;
using Lisa.Excelsis.WebApi;

namespace test
{
    public class TestController : Controller
    {
        [HttpGet]
        [Authorize(ActiveAuthenticationSchemes = "Bearer", Roles = "student,teacher")]
        [Route("api/secured/ping")]
        public object SecuredPing()
        {
            return new
            {
                IsTeacher = User.IsInRole("teacher"),
                IsStudent = User.IsInRole("student"),
                IsAuthenticated = User.Identity.IsAuthenticated,
                Profile = Startup.Profile
            };
        }

        [HttpGet]
        [Authorize(ActiveAuthenticationSchemes = "Bearer", Roles = "teacher,student")]
        [Route("api/secured/pong")]
        public object SecuredPong()
        {
            return new
            {
                IsTeacher = User.IsInRole("teacher"),
                IsStudent = User.IsInRole("student"),
                IsAuthenticated = User.Identity.IsAuthenticated,
                Profile = Startup.Profile
            };
        }
    }
}
