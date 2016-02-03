using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Authorization;

namespace Lisa.Excelsis.WebApi
{
    public class ProfileController : Controller
    {
        [HttpGet]
        [Authorize(ActiveAuthenticationSchemes = "Bearer", Roles = "teacher,student")]
        [Route("[controller]")]
        public object Get()
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