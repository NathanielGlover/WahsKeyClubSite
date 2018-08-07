using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WahsKeyClubSite.Areas.Identity.Data;
using WahsKeyClubSite.Models;

namespace WahsKeyClubSite.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserDbContext context;
        private readonly ServiceHoursDbContext hoursContext;
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;

        public UsersController(UserDbContext context, UserManager<User> userManager, SignInManager<User> signInManager, ServiceHoursDbContext hoursContext)
        {
            this.context = context;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.hoursContext = hoursContext;
        }

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
            
            return View(await context.Users.ToListAsync());
        }

        // GET: Hours/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if(!signInManager.IsSignedIn(User))
            {
                return RedirectToPage("/Account/Login", new { area = "Identity"});
            }

            if(!userManager.GetUserAsync(User).Result.IsAdmin())
            {
                return RedirectToPage("/Account/AccessDenied", new { area = "Identity"});
            }
            
            if(id == "")
            {
                return NotFound();
            }

            var userId = await context.Users.FirstOrDefaultAsync(m => m.Id == id);
            if(userId == null)
            {
                return NotFound();
            }

            return View(userId);
        }

        // POST: Hours/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if(!signInManager.IsSignedIn(User))
            {
                return RedirectToPage("/Account/Login", new { area = "Identity"});
            }

            if(!userManager.GetUserAsync(User).Result.IsAdmin())
            {
                return RedirectToPage("/Account/AccessDenied", new { area = "Identity"});
            }
            
            var user = await context.Users.FindAsync(id);
            var hours = from entry in hoursContext.ServiceHours.ToList() where entry.UserId == id select entry;
            hoursContext.ServiceHours.RemoveRange(hours);
            await hoursContext.SaveChangesAsync();
            context.Users.Remove(user);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ChangeRole(string id)
        {
            if(!signInManager.IsSignedIn(User))
            {
                return RedirectToPage("/Account/Login", new { area = "Identity"});
            }

            if(!userManager.GetUserAsync(User).Result.IsAdmin())
            {
                return RedirectToPage("/Account/AccessDenied", new { area = "Identity"});
            }
            
            if(id == "")
            {
                return NotFound();
            }

            var user = await context.Users.FirstOrDefaultAsync(m => m.Id == id);
            if(user == null)
            {
                return NotFound();
            }

            return View(new InputModel {Id = user.Id});
        }

        public class InputModel
        {
            [Required]
            public AccountType AccountType { get; set; }
            
            public string Id { get; set; }
        }

        [HttpPost, ActionName("ChangeRole")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRoleConfirmed([Bind("Id,AccountType")] InputModel input)
        {
            if(!signInManager.IsSignedIn(User))
            {
                return RedirectToPage("/Account/Login", new { area = "Identity"});
            }

            if(!userManager.GetUserAsync(User).Result.IsAdmin())
            {
                return RedirectToPage("/Account/AccessDenied", new { area = "Identity"});
            }

            var user = await userManager.FindByIdAsync(input.Id);
            user.AccountType = input.AccountType;
            Console.WriteLine(input.AccountType);

            context.Users.Update(user);
            
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}