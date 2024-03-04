using Microsoft.AspNetCore.Mvc;

namespace AhercodeWebAPI.HR.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
