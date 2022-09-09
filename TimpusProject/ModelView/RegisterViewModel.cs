using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace TimpusProject.ModelView
{
    public class RegisterViewModel
    {
        [Key]
        public int CustomerId { get; set; }

        [Display(Name = "Full name")]
        [Required(ErrorMessage = "Please input your full name")]
        public string FullName { get; set; }

        [Display(Name = "User name")]
        [Required(ErrorMessage = "Please input your username")]
        public string Username { get; set; }

        [MaxLength(150)]
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Please input your Email")]
        [DataType(DataType.Text)]
        [Remote(action: "ValidateEmail", controller: "Accounts")]
        public string Email { get; set; }

        [MaxLength(11)]
        [Required(ErrorMessage = "Please input your phone number")]
        [Display(Name = "Phone number")]
        [DataType(DataType.PhoneNumber)]
        [Remote(action: "ValidatePhone", controller: "Accounts")]
        public string Phone { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Please input your password")]
        [MinLength(5, ErrorMessage = "Password need at least 5 charaters.")]
        public string Password { get; set; }

        [MinLength(5, ErrorMessage = "Password need at least 5 charaters.")]
        [Display(Name = "Nhập lại mật khẩu")]
        [Compare("Password", ErrorMessage = "Re-enter incorrect password")]
        public string ConfirmPassword { get; set; }
    }
}
