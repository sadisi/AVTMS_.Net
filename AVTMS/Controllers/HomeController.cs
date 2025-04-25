using System.Diagnostics;
using AVTMS.Data;
using AVTMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AVTMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger , AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            //get the registered number of vehicles
            var vehicleCount = _context.Vehicles.Count();
            ViewBag.VehicleCount = vehicleCount;

            //get the registered number of vehicle owners
            var VehicleOwnersCount = _context.VehicleOwner.Count();
            ViewBag.VehicleOwnersCount = VehicleOwnersCount;

            //get the registered number of system users
            var systemRegisteredUsersCount = _context.Users.Count();
            ViewBag.systemRegisteredUsersCount = systemRegisteredUsersCount;
            return View();
        }




        [Authorize] // when loged in then show privacy page
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
