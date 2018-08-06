using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WahsKeyClubSite.Areas.Identity.Data;

namespace WahsKeyClubSite.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<User> _userManager;

        public ConfirmEmailModel(UserManager<User> userManager) => _userManager = userManager;

        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            Console.WriteLine("f");
            Console.WriteLine("f");
            Console.WriteLine("f");
            Console.WriteLine("f");
            Console.WriteLine("f");
            Console.WriteLine("f");
            Console.WriteLine("f");
            Console.WriteLine("f");
            Console.WriteLine("f");
            Console.WriteLine("f");
            Console.WriteLine("f");
            Console.WriteLine("f");
            Console.WriteLine("f");
            Console.WriteLine("f");
            Console.WriteLine("f");
            Console.WriteLine("f");
            Console.WriteLine("f");
            Console.WriteLine("f");
            Console.WriteLine("f");
            Console.WriteLine("f");
            Console.WriteLine("f");
            Console.WriteLine("f");
            Console.WriteLine("f");
            Console.WriteLine("f");
            Console.WriteLine("f");
            Console.WriteLine("f");
            if(userId == null || code == null)
            {
                Console.WriteLine("gay");
                Console.WriteLine(userId);
                Console.WriteLine(code);

                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if(user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if(!result.Succeeded)
            {
                throw new InvalidOperationException($"Error confirming email for user with ID '{userId}':");
            }

            return Page();
        }
    }
}