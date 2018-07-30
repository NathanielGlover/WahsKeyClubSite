using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WahsKeyClubSite.Models;

namespace WahsKeyClubSite.Controllers
{
    public class HomeController : Controller
    {
        public static bool SignedIn = false;
        
        public static User CurrentUser => new User();
        
        public IActionResult Index() => View();

        public IActionResult Calendar() => View();

        public IActionResult Contact() => View();

        public IActionResult Error() => View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }
}