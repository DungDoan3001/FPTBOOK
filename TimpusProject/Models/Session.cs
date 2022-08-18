using System;
using System.Collections.Generic;

#nullable disable

namespace TimpusProject.Models
{
    public partial class Session
    {
        public Session()
        {
            Carts = new HashSet<Cart>();
        }

        public int SessionId { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? CustomerId { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual ICollection<Cart> Carts { get; set; }
    }
}
