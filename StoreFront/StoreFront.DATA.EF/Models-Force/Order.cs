using System;
using System.Collections.Generic;

namespace StoreFront.DATA.EF.Models-Force
{
    public partial class Order
    {
        public Order()
        {
            OrderProducts = new HashSet<OrderProduct>();
        }

        public int OrderId { get; set; }
        public string CustomerId { get; set; } = null!;
        public DateTime OrderDate { get; set; }
        public string ShipToName { get; set; } = null!;
        public string ShipCity { get; set; } = null!;
        public string ShipState { get; set; } = null!;
        public string ShipZip { get; set; } = null!;

        public virtual CustomerDetail Customer { get; set; } = null!;
        public virtual ICollection<OrderProduct> OrderProducts { get; set; }
    }
}
