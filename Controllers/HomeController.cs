using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WahsKeyClubSite.Models;

namespace WahsKeyClubSite.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View(HttpContext.Request.IsMobileBrowser());

        public IActionResult Calendar() => View(HttpContext.Request.IsMobileBrowser());

        public IActionResult Contact() => View();

        public IActionResult Error() => View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }
}