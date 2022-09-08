using System;
using System.Collections.Generic;

#nullable disable

namespace TimpusProject.Models
{
    public partial class Product
    {
        public Product()
        {
            AuthorProducts = new HashSet<AuthorProduct>();
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public int? CatId { get; set; }
        public decimal? Price { get; set; }
        public string Thumb { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public bool? BestSellers { get; set; }
        public bool? HomeFlag { get; set; }
        public bool? Active { get; set; }
        public int? UnitInStock { get; set; }
        public string SmallDescription { get; set; }
        public string Isbn { get; set; }
        public int? PublisherId { get; set; }

        public virtual Category Cat { get; set; }
        public virtual Publisher Publisher { get; set; }
        public virtual ICollection<AuthorProduct> AuthorProducts { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
