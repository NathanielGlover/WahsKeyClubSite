using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace WahsKeyClubSite.Areas.Identity.Data
{
    public enum AccountType
    {
        Member,
        Admin,
        Developer
    }

    public enum Grade
    {
        Freshman = 0,
        Sophomore = 1,
        Junior = 2,
        Senior = 3
    }

    public class User : IdentityUser
    {
        [PersonalData]
        public DateTime CreationDate { get; set; }

        [PersonalData]
        public AccountType AccountType { get; set; } = AccountType.Member;

        [PersonalData]
        public string Name { get; set; }

        [PersonalData]
        public Grade Grade { get; set; }

        public bool IsMember() => !IsAdmin();

        public bool IsAdmin() => AccountType == AccountType.Admin || AccountType == AccountType.Developer;

        public bool IsDeveloper() => AccountType == AccountType.Developer;

        public bool IsQualified(AccountType qualificationLevel)
        {
            switch(qualificationLevel)
            {
                case AccountType.Member:
                    return true;
                case AccountType.Admin:
                    return IsAdmin();
                case AccountType.Developer:
                    return IsDeveloper();
                default:
                    return false;
            }
        }
    }
}