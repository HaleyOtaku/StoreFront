using System;
using System.Collections.Generic;

namespace StoreFront.DATA.EF.Models
{
    public partial class OrderProduct
    {
        public int OrderProductsId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal ProductPrice { get; set; }

        public virtual Order Order { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
    }
}
