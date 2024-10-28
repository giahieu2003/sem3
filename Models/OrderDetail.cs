using System;
using System.Collections.Generic;

namespace Sem3.Models
{
    public partial class OrderDetail
    {
        public int OrderDetailId { get; set; }
        public string? OrderId { get; set; }
        public string? ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public virtual Order? Order { get; set; }
        public virtual Product? Product { get; set; }
    }
}
