using System;
using System.Collections.Generic;

#nullable disable

namespace TimpusProject.Models
{
    public partial class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }

        public int CatId { get; set; }
        public string CatName { get; set; }
        public string Description { get; set; }
        public int? Ordering { get; set; }
        public bool? Published { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
