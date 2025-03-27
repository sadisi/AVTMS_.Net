using Microsoft.AspNetCore.Mvc;

namespace AVTMS.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
    }
}
