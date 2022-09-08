using System;
using System.Collections.Generic;

#nullable disable

namespace TimpusProject.Models
{
    public partial class AuthorProduct
    {
        public int AuthorId { get; set; }
        public int ProductId { get; set; }

        public virtual Author Author { get; set; }
        public virtual Product Product { get; set; }
    }
}
