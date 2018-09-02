using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WahsKeyClubSite.Areas.Identity.Data;
using WahsKeyClubSite.Models;

namespace WahsKeyClubSite.Controllers
{
    public class HoursController : Controller
    {
        private readonly ServiceHoursDbContext context;
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;

        private readonly List<DateTime> quarters;

        public HoursController(ServiceHoursDbContext context, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            this.context = context;
            this.userManager = userManager;
            this.signInManager = signInManager;

            int currentSchoolYear = DateTime.Now.Year;

            if(new DateTime(currentSchoolYear, 6, 7) > DateTime.Now)
            {
                currentSchoolYear -= 1;
            }

            //TODO: Read these from database
            quarters = new List<DateTime>
            {
                new DateTime(currentSchoolYear, 6, 7),
                new DateTime(currentSchoolYear, 11, 1),
                new DateTime(currentSchoolYear + 1, 1, 18),
                new DateTime(currentSchoolYear + 1, 3, 28),
                new DateTime(currentSchoolYear + 1, 6, 7)
            };
        }

        public IActionResult UnverifiedAccount() => View();

        public async Task<IActionResult> QuarterlyTotals()
        {
            if(!signInManager.IsSignedIn(User))
            {
                return RedirectToPage("/Account/Login", new {area = "Identity"});
            }

            if(!userManager.GetUserAsync(User).Result.IsAdmin())
            {
                return RedirectToPage("/Account/AccessDenied", new {area = "Identity"});
            }

            var hours = await context.ServiceHours.ToListAsync();

            var model = new List<double>(4);

            for(int i = 1; i < quarters.Count; i++)
            {
                var total = (from entry in hours where entry.DateOfActivity >= quarters[i - 1] && entry.DateOfActivity < quarters[i] select entry.Hours)
                    .Sum();
                model.Add(total);
            }

            ViewData["Year"] = 2018;

            return View(model);
        }

        public async Task<IActionResult> MonthlyTotals()
        {
            if(!signInManager.IsSignedIn(User))
            {
                return RedirectToPage("/Account/Login", new {area = "Identity"});
            }

            if(!userManager.GetUserAsync(User).Result.IsAdmin())
            {
                return RedirectToPage("/Account/AccessDenied", new {area = "Identity"});
            }

            var hours = await context.ServiceHours.ToListAsync();

            var beginning = new DateTime(2018, 6, 1);

            var model = new Dictionary<DateTime, double>();

            while(beginning < DateTime.Today)
            {
                var tempBeginning = beginning;
                var nextMonth = beginning.AddMonths(1);

                var total = (from entry in hours where entry.DateOfActivity >= tempBeginning && entry.DateOfActivity < nextMonth select entry.Hours)
                    .Sum();

                model.Add(beginning, total);

                beginning = nextMonth;
            }

            return View(model);
        }

        public async Task<IActionResult> UserTotals()
        {
            if(!signInManager.IsSignedIn(User))
            {
                return RedirectToPage("/Account/Login", new {area = "Identity"});
            }

            if(!userManager.GetUserAsync(User).Result.IsAdmin())
            {
                return RedirectToPage("/Account/AccessDenied", new {area = "Identity"});
            }

            var hours = await context.ServiceHours.ToListAsync();
            var totalHours = new Dictionary<User, double>(userManager.Users.Count());

            foreach(var user in userManager.Users)
            {
                totalHours.Add(user, (from entry in hours where entry.UserId == user.Id select entry.Hours).Sum());
            }

            return View(new Dictionary<User, double>(totalHours.OrderBy(pair => pair.Key.Name)));
        }

        public async Task<IActionResult> UserQuarterlyTotals(string id)
        {
            if(!signInManager.IsSignedIn(User))
            {
                return RedirectToPage("/Account/Login", new {area = "Identity"});
            }

            string userId = id ?? userManager.GetUserAsync(User).Result.Id;

            ViewData["UserID"] = userId;

            if(userId == userManager.GetUserId(User))
            {
                ViewData["Header"] = "View, edit, and delete your service hour entries.";
                ViewData["YourHours"] = true;
            }
            else
            {
                if(!userManager.GetUserAsync(User).Result.IsAdmin())
                {
                    return RedirectToPage("/Account/AccessDenied", new {area = "Identity"});
                }

                ViewData["Header"] =
                    $"View and delete {userManager.FindByIdAsync(userId).Result.Name}'s service hour entries. This should only be visible to users with admin privileges.";
                ViewData["YourHours"] = false;
            }

            var hours = (from entry in await context.ServiceHours.ToListAsync() where entry.UserId == userId select entry).ToList();

            var model = new List<double>(4);

            for(int i = 1; i < quarters.Count; i++)
            {
                var total = (from entry in hours where entry.DateOfActivity >= quarters[i - 1] && entry.DateOfActivity < quarters[i] select entry.Hours)
                    .Sum();
                model.Add(total);
            }

            ViewData["Year"] = 2018;

            return View(model);
        }

        public async Task<IActionResult> UserMonthlyTotals(string id)
        {
            if(!signInManager.IsSignedIn(User))
            {
                return RedirectToPage("/Account/Login", new {area = "Identity"});
            }

            string userId = id ?? userManager.GetUserAsync(User).Result.Id;

            ViewData["UserID"] = userId;

            if(userId == userManager.GetUserId(User))
            {
                ViewData["Header"] = "View, edit, and delete your service hour entries.";
                ViewData["YourHours"] = true;
            }
            else
            {
                if(!userManager.GetUserAsync(User).Result.IsAdmin())
                {
                    return RedirectToPage("/Account/AccessDenied", new {area = "Identity"});
                }

                ViewData["Header"] =
                    $"View and delete {userManager.FindByIdAsync(userId).Result.Name}'s service hour entries. This should only be visible to users with admin privileges.";
                ViewData["YourHours"] = false;
            }

            var hours = (from entry in await context.ServiceHours.ToListAsync() where entry.UserId == userId select entry).ToList();

            var beginning = new DateTime(2018, 6, 1);

            var model = new Dictionary<DateTime, double>();

            while(beginning < DateTime.Today)
            {
                var tempBeginning = beginning;
                var nextMonth = beginning.AddMonths(1);

                var total = (from entry in hours where entry.DateOfActivity >= tempBeginning && entry.DateOfActivity < nextMonth select entry.Hours)
                    .Sum();

                model.Add(beginning, total);

                beginning = nextMonth;
            }

            return View(model);
        }

        public async Task<IActionResult> ViewHours(string id)
        {
            if(!signInManager.IsSignedIn(User))
            {
                return RedirectToPage("/Account/Login", new {area = "Identity"});
            }

            string userId = id ?? userManager.GetUserAsync(User).Result.Id;

            ViewData["UserID"] = userId;

            if(userId == userManager.GetUserId(User))
            {
                ViewData["Header"] = "View, edit, and delete your service hour entries.";
                ViewData["YourHours"] = true;
            }
            else
            {
                if(!userManager.GetUserAsync(User).Result.IsAdmin())
                {
                    return RedirectToPage("/Account/AccessDenied", new {area = "Identity"});
                }

                ViewData["Header"] =
                    $"View and delete {userManager.FindByIdAsync(userId).Result.Name}'s service hour entries. This should only be visible to users with admin privileges.";
                ViewData["YourHours"] = false;
            }

            return View(from entry in await context.ServiceHours.ToListAsync() where entry.UserId == userId select entry);
        }

        // GET: Hours
        public async Task<IActionResult> Index()
        {
            if(!signInManager.IsSignedIn(User))
            {
                return RedirectToPage("/Account/Login", new {area = "Identity"});
            }

            if(!userManager.GetUserAsync(User).Result.IsAdmin())
            {
                return RedirectToPage("/Account/AccessDenied", new {area = "Identity"});
            }

            return View(await context.ServiceHours.ToListAsync());
        }

        public IActionResult Edit(DateTime date, double hours, string name)
        {
            if(!signInManager.IsSignedIn(User))
            {
                return RedirectToPage("/Account/Login", new {area = "Identity"});
            }

            ViewData["Date"] = date.ToString("yyyy-MM-dd");
            ViewData["Hours"] = hours;
            ViewData["Name"] = name;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("ID,DateOfActivity,Hours,Activity")]
            ServiceHours serviceHours)
        {
            if(ModelState.IsValid)
            {
                if(!signInManager.IsSignedIn(User))
                {
                    return RedirectToPage("/Account/Login", new {area = "Identity"});
                }

                var hours = await context.ServiceHours.FindAsync(serviceHours.ID);
                hours.DateOfActivity = serviceHours.DateOfActivity;
                hours.Hours = serviceHours.Hours;
                hours.Activity = serviceHours.Activity;

                await context.SaveChangesAsync();
                return RedirectToAction(nameof(ViewHours));
            }

            return View(serviceHours);
        }

        // GET: Hours/Create
        public IActionResult Submit()
        {
            if(!signInManager.IsSignedIn(User))
            {
                return RedirectToPage("/Account/Login", new {area = "Identity"});
            }

            if(!userManager.GetUserAsync(User).Result.EmailConfirmed)
            {
                return RedirectToAction("UnverifiedAccount", "Hours");
            }

            return View();
        }

        // POST: Hours/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit([Bind("ID,DateOfActivity,Hours,Activity")]
            ServiceHours serviceHours)
        {
            serviceHours.DateSubmitted = DateTime.Now;

            if(ModelState.IsValid)
            {
                if(!signInManager.IsSignedIn(User))
                {
                    return RedirectToPage("/Account/Login", new {area = "Identity"});
                }

                if(!userManager.GetUserAsync(User).Result.EmailConfirmed)
                {
                    return RedirectToAction("UnverifiedAccount", "Hours");
                }

                serviceHours.UserId = userManager.GetUserAsync(User).Result.Id;

                context.Add(serviceHours);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(ViewHours));
            }

            return View(serviceHours);
        }

        // GET: Hours/Create
        public IActionResult SubmitDeveloper()
        {
            if(!signInManager.IsSignedIn(User))
            {
                return RedirectToPage("/Account/Login", new {area = "Identity"});
            }

            if(!userManager.GetUserAsync(User).Result.IsDeveloper())
            {
                return RedirectToPage("/Account/AccessDenied", new {area = "Identity"});
            }

            return View();
        }

        // POST: Hours/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitDeveloper([Bind("ID,UserId,DateOfActivity,Hours,Activity")]
            ServiceHours serviceHours)
        {
            serviceHours.DateSubmitted = DateTime.Now;

            if(ModelState.IsValid)
            {
                if(!signInManager.IsSignedIn(User))
                {
                    return RedirectToPage("/Account/Login", new {area = "Identity"});
                }

                if(!userManager.GetUserAsync(User).Result.IsDeveloper())
                {
                    return RedirectToPage("/Account/AccessDenied", new {area = "Identity"});
                }

                context.Add(serviceHours);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(ViewHours));
            }

            return View(serviceHours);
        }

        // GET: Hours/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if(!signInManager.IsSignedIn(User))
            {
                return RedirectToPage("/Account/Login", new {area = "Identity"});
            }

            if(!userManager.GetUserAsync(User).Result.IsAdmin())
            {
                return RedirectToPage("/Account/AccessDenied", new {area = "Identity"});
            }

            if(id == null)
            {
                return NotFound();
            }

            var serviceHoursId = await context.ServiceHours.FirstOrDefaultAsync(m => m.ID == id);
            if(serviceHoursId == null)
            {
                return NotFound();
            }

            return View(serviceHoursId);
        }

        // POST: Hours/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if(!signInManager.IsSignedIn(User))
            {
                return RedirectToPage("/Account/Login", new {area = "Identity"});
            }

            if(!userManager.GetUserAsync(User).Result.IsAdmin())
            {
                return RedirectToPage("/Account/AccessDenied", new {area = "Identity"});
            }

            var serviceHours = await context.ServiceHours.FindAsync(id);
            context.ServiceHours.Remove(serviceHours);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(ViewHours));
        }

        private bool ServiceHoursExists(int id)
        {
            return context.ServiceHours.Any(e => e.ID == id);
        }
    }
}