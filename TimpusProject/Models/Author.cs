using System;
using System.Collections.Generic;

#nullable disable

namespace TimpusProject.Models
{
    public partial class Author
    {
        public Author()
        {
            AuthorProducts = new HashSet<AuthorProduct>();
        }

        public int AuthorId { get; set; }
        public string FullName { get; set; }
        public string Biography { get; set; }
        public string Avatar { get; set; }

        public virtual ICollection<AuthorProduct> AuthorProducts { get; set; }
    }
}
