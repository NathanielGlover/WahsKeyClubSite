using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WahsKeyClubSite.Models;

namespace WahsKeyClubSite.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserContext context;

        public UsersController(UserContext context) => this.context = context;

        // GET: Users
        public async Task<IActionResult> Index() => View(await context.User.ToListAsync());

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await context.User
                .FirstOrDefaultAsync(m => m.ID == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        public IActionResult SignIn() => View();

        public IActionResult CreateMember() => View();
        
        // GET: Users/Create
        public IActionResult Create() => View();

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Email,Password,FirstName,LastName,Grade,AccountType")] User user)
        {
            if (ModelState.IsValid)
            {
                user.CreationDate = DateTime.Now;
                context.Add(user);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await context.User.FirstOrDefaultAsync(m => m.ID == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await context.User.FindAsync(id);
            context.User.Remove(user);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return context.User.Any(e => e.ID == id);
        }
    }
}
