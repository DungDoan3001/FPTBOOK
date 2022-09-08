using System;
using System.ComponentModel.DataAnnotations;

namespace TimpusProject.ModelView
{
    public class LoginViewModel
    {
        [Key]
        [MaxLength(100)]
        [Required(ErrorMessage = ("Please input Email"))]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Wrong Validation Email!")]
        public string UserName { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Please input Password")]
        [MinLength(5, ErrorMessage = "Password need at least 5 charaters.")]
        public string Password { get; set; }
    }
}
