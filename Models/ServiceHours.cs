using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WahsKeyClubSite.Models
{
    public class PosNumberNoZeroAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if(value == null)
            {
                return true;
            }

            if(double.TryParse(value.ToString(), out double number))
            {
                if(Math.Abs(number) < 0.001)
                    return false;

                if(number > 0)
                    return true;
            }

            return false;
        }
    }

    public class ServiceHours
    {
        // ReSharper disable once InconsistentNaming
        public int ID { get; set; }
        
        public string UserId { get; set; }
        
        [Display(Name = "Date Submitted")]
        public DateTime DateSubmitted { get; set; }

        [Required(ErrorMessage = "You must enter a date for the service activity.")]
        [Display(Name = "Date of Activity")]
        [DataType(DataType.Date)]
        public DateTime DateOfActivity { get; set; }

        [Required(ErrorMessage = "You must enter a positive quantity of hours.")]
        [Display(Name = "Total Hours")]
        [DataType(DataType.Duration)]
        [PosNumberNoZero]
        public double Hours { get; set; }

        [Required(ErrorMessage = "You must specify the activity (i.e. Crozet Elementary Bake Sale, Book Buddies, etc.)")]
        [Display(Name = "Service Activity")]
        public string Activity { get; set; }
    }
}