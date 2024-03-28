using System;
using System.Collections.Generic;

namespace StoreFront.DATA.EF.Models-Force
{
    public partial class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public string? CategoryDescript { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
