using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WahsKeyClubSite.Areas.Identity.Data;
using WahsKeyClubSite.Models;

namespace WahsKeyClubSite.Backend
{
    public class RequestResult
    {
        public RequestResult(IEnumerable<ServiceHours> result, ValidationStatus validationStatus)
        {
            Result = result;
            ValidationStatus = validationStatus;
        }
        
        public IEnumerable<ServiceHours> Result { get; }
        public ValidationStatus ValidationStatus { get; }

        public bool IsValid => ValidationStatus == ValidationStatus.Valid;
    }
    
    public class HoursManager
    {
        public static int CurrentSchoolYear
        {
            get
            {
                var currentTime = DateTime.Now;
                return (new DateTime(currentTime.Year, 6, 1) > DateTime.Now ? currentTime.AddYears(-1) : currentTime).Year;
            }
        }

        private ServiceHoursDbContext Context { get; }
        private UserManager<User> UserManager { get; }
        private SignInManager<User> SignInManager { get; }

        public HoursManager(ServiceHoursDbContext context, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            Context = context;
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public async Task<bool> ValidateUserEquality(User user, ClaimsPrincipal requester) => (await UserManager.GetUserAsync(requester)).Id == user.Id;

        public bool ValidateSignInStatus(ClaimsPrincipal requester) => SignInManager.IsSignedIn(requester);

        public async Task<bool> ValidateUserQualifications(ClaimsPrincipal requester, AccountType accessLevel) =>
            (await UserManager.GetUserAsync(requester)).IsQualified(accessLevel);

        public async Task<ValidationStatus> ValidateUserSpecificRequest(User user, ClaimsPrincipal requester)
        {
            if(!ValidateSignInStatus(requester)) return ValidationStatus.NotSignedIn;
            if(await ValidateUserEquality(user, requester)) return ValidationStatus.Valid;
            
            if(await ValidateUserQualifications(requester, user.IsAdmin() ? AccountType.Developer : AccountType.Admin))
            {
                return user.IsDeveloper() ? ValidationStatus.NotQualified : ValidationStatus.Valid;
            }

            return ValidationStatus.NotQualified;
        }

        public async Task<ValidationStatus> ValidateFullReadAccessRequest(ClaimsPrincipal requester)
        {
            if(!ValidateSignInStatus(requester)) return ValidationStatus.NotSignedIn;
            if(await ValidateUserQualifications(requester, AccountType.Admin)) return ValidationStatus.Valid;

            return ValidationStatus.NotQualified;
        }

        public async Task<RequestResult> RequestUserSpecificHours(User user, ClaimsPrincipal requester)
        {
            var validationStatus = await ValidateUserSpecificRequest(user, requester);

            var allHours = await Context.ServiceHours.ToListAsync();
            var result = from hours in allHours where hours.UserId == user.Id select hours;
            
            return new RequestResult(result, validationStatus);
        }

        public async Task<RequestResult> RequestUserSpecificHours(ClaimsPrincipal userAndRequester) => 
            await RequestUserSpecificHours(await UserManager.GetUserAsync(userAndRequester), userAndRequester);

        public async Task<RequestResult> RequestReadAllHours(ClaimsPrincipal requester)
        {
            var validationStatus = await ValidateFullReadAccessRequest(requester);

            var allHours = await Context.ServiceHours.ToListAsync();
            return new RequestResult(allHours, validationStatus);
        }
    }
}