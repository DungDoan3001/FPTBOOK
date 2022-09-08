using System;
using System.Collections.Generic;

#nullable disable

namespace TimpusProject.Models
{
    public partial class Publisher
    {
        public Publisher()
        {
            Products = new HashSet<Product>();
        }

        public int PublisherId { get; set; }
        public string Avatar { get; set; }
        public string FullName { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
