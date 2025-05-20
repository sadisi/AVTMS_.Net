using System.Diagnostics;
using System.Globalization;
using AVTMS.Data;
using AVTMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;



namespace AVTMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
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

            //get viloation counts
            var violationCount = _context.ViolationEmails.Count();
            ViewBag.ViolationCount = violationCount;


            //viloation presentage
            double violationPercentage = 0;
             var totalVehicles = _context.Vehicles.Count();

            if (totalVehicles > 0)
            {
                violationPercentage = (double)violationCount / totalVehicles * 100;
            }

            ViewBag.ViolationPercentage = Math.Round(violationPercentage, 2);



            //chart data


            var vehicleData = _context.Vehicles
        .GroupBy(v => v.CreatedOn.Month)
        .Select(g => new
        {
            Month = g.Key,
            Count = g.Count()
        })
        .OrderBy(g => g.Month)
        .ToList();

            var labels = vehicleData.Select(v => CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(v.Month)).ToList();
            var counts = vehicleData.Select(v => v.Count).ToList();

            ViewBag.VehicleRegLabels = JsonConvert.SerializeObject(labels);
            ViewBag.VehicleRegCounts = JsonConvert.SerializeObject(counts);

            // Step 5: Return the view with the data available in ViewBag



            //

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



        //Search bar 
        private readonly List<(string Title, string Url)> _pages = new List<(string, string)>
    {
        //HomePage related
        ("Home", "/Home/Index"),
        ("Contact", "/Home/Contact"),
        ("Privacy", "/Home/Privacy"),

        //Registered User related
        ("Registered Users", "/Account/RegisteredUsers"),
        ("Edit Registered Users", "/Account/EditUser"),
       
         //Base User related
        ("Base Users", "/BaseUsers/Index"),
        ("Add Base Users", "/BaseUsers/Create"),

        //Authorized User related
        ("Authorized Users", "/AuthUsers/Index"),
        ("Add Authorized Users", "/AuthUsers/Create"),

        //Vehicle related
        ("Vehicles", "/Vehicles/Index"),
        ("Add Vehicles", "/Vehicles/Create"),

        //Admin related
        ("Admin", "/Admins/Index"),

        //Vehicle Owner related
        ("Vehicle Owners", "/VehicleOwners/Index"),
        ("Add Vehicle Owners", "/VehicleOwners/Create"),

        //Vehicle Detection related
        ("Vehicles Detection", "/VehicleDetection/Upload"),

        //Vehicle Notes related
        ("Vehicle Notes", "/VehicleNotes/Index"),
        ("Add Vehicle Notes", "/VehicleNotes/Create"),

        //Live CCTV related
        ("Live CCTV", "/VehicleDetection/LiveCCTVStream"),
        
    };

        [HttpGet]
        public IActionResult SearchPages(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Content("");

            var matches = _pages
                .Where(p => p.Title.Contains(query, StringComparison.OrdinalIgnoreCase))
                .Select(p => $"<li class='list-group-item'><a href='{p.Url}'>{p.Title}</a></li>")
                .ToList();

            if (matches.Count == 0)
                return Content("<li class='list-group-item'>No results found</li>");

            return Content(string.Join("", matches));
        }




        //Chart
    }
}
