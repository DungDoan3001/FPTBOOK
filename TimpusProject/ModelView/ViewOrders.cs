using System;
using System.Collections.Generic;
using TimpusProject.Models;
namespace TimpusProject.ModelView
{
    public class ViewOrders
    {
        public Order Orders { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
    }
}
