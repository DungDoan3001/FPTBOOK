using System;
using System.ComponentModel.DataAnnotations;
namespace TimpusProject.ModelView
{
    public class ChangePasswordVM
    {
        [Key]
        public int CustomerId { get; set; }

        [Display(Name = "Your Current password")]
        [Required(ErrorMessage = "Please enter your current password")]
        public string PasswordNow { get; set; }

        [Display(Name = "New password")]
        [Required(ErrorMessage = "Please enter your new password")]
        [MinLength(5, ErrorMessage = "Password must be at least 5 characters")]
        public string Password { get; set; }

        [MinLength(5, ErrorMessage = "Password must be at least 5 characters")]
        [Display(Name = "Re-enter password")]
        [Compare("Password", ErrorMessage = "Re-enter incorrect password")]
        public string ConfirmPassword { get; set; }
    }
}
