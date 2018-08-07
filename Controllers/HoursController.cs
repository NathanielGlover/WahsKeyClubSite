using System;
using System.Collections.Generic;
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

        public HoursController(ServiceHoursDbContext context, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            this.context = context;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public async Task<IActionResult> ViewHours()
        {
            if(!signInManager.IsSignedIn(User))
            {
                return RedirectToPage("/Account/Login", new { area = "Identity"});
            }

            string userId = userManager.GetUserAsync(User).Result.Id;
            
            return View(from entry in await context.ServiceHours.ToListAsync() where entry.UserId == userId select entry);
        }

        // GET: Hours
        public async Task<IActionResult> Index()
        {
            if(!signInManager.IsSignedIn(User))
            {
                return RedirectToPage("/Account/Login", new { area = "Identity"});
            }

            if(!userManager.GetUserAsync(User).Result.IsAdmin())
            {
                return RedirectToPage("/Account/AccessDenied", new { area = "Identity"});
            }

            return View(await context.ServiceHours.ToListAsync());
        }

        public IActionResult Edit()
        {
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
                    return RedirectToPage("/Account/Login", new { area = "Identity"});
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
                    return RedirectToPage("/Account/Login", new { area = "Identity"});
                }

                serviceHours.UserId = userManager.GetUserAsync(User).Result.Id;

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
                return RedirectToPage("/Account/Login", new { area = "Identity"});
            }

            if(!userManager.GetUserAsync(User).Result.IsAdmin())
            {
                return RedirectToPage("/Account/AccessDenied", new { area = "Identity"});
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
                return RedirectToPage("/Account/Login", new { area = "Identity"});
            }

            if(!userManager.GetUserAsync(User).Result.IsAdmin())
            {
                return RedirectToPage("/Account/AccessDenied", new { area = "Identity"});
            }
            
            var serviceHours = await context.ServiceHours.FindAsync(id);
            context.ServiceHours.Remove(serviceHours);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ServiceHoursExists(int id)
        {
            return context.ServiceHours.Any(e => e.ID == id);
        }
    }
}