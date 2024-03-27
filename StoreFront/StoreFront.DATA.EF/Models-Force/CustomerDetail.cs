using System;
using System.Collections.Generic;

namespace StoreFront.DATA.EF.Models-Force
{
    public partial class CustomerDetail
    {
        public CustomerDetail()
        {
            Orders = new HashSet<Order>();
        }

        public string CustomerId { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string City { get; set; } = null!;
        public string State { get; set; } = null!;
        public string Zip { get; set; } = null!;
        public string? Phone { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
