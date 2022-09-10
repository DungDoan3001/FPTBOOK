using System;
using System.ComponentModel.DataAnnotations;

namespace TimpusProject.ModelView
{
    public class BuyVM
    {
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Please input your Full Name")]
        public string FullName { get; set; }
        public string Email { get; set; }
        [Required(ErrorMessage = "Please input your Phone Number")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Please input your Delivery Address")]
        public string Address { get; set; }
        public int PaymentID { get; set; }
        public string Note { get; set; }
    }
}
