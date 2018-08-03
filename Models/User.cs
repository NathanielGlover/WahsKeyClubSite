using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Storage;

namespace WahsKeyClubSite.Models
{
    public enum Grade
    {
        Freshman,
        Sophomore,
        Junior,
        Senior
    }

    public enum AccountType
    {
        Member,
        Admin,
        Developer
    }
    
    public class User
    {
        // ReSharper disable once InconsistentNaming
        public int ID { get; set; }
        
        [DataType(DataType.DateTime)]
        public string CreationDate { get; set; }

        public AccountType AccountType { get; set; } = AccountType.Member;

        [Required(ErrorMessage = "Email address is required.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Grade is required.")]
        public Grade Grade { get; set; }

        public bool IsMember() => !IsAdmin();

        public bool IsAdmin() => AccountType == AccountType.Admin || AccountType == AccountType.Developer;

        public bool IsDeveloper() => AccountType == AccountType.Developer;
    }
}