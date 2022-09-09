using System;
using TimpusProject.Models;

namespace TimpusProject.ModelView
{
    public class CartItem
    {
        public Product product { get; set; }
        public int amount { get; set; }
        public decimal TotalMoney => amount * product.Price.Value;
    }
}
