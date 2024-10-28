using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sem3.Models
{
    public partial class Product
    {
        public Product()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public string ProductId { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public string? Description { get; set; }
        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
        public decimal Price { get; set; }
        public string? Image { get; set; }
        public int? CategoryId { get; set; }

        public virtual Category? Category { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

    }
}
