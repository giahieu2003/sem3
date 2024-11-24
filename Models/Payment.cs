using System;
using System.Collections.Generic;

namespace Sem3.Models
{
    public partial class Payment
    {
        public int PaymentId { get; set; }
        public string? OrderId { get; set; }
        public string? PaymentType { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string? PaymentStatus { get; set; }
        public decimal Amount { get; set; }
        public string? CardNumber { get; set; }
        public string? ChequeNumber { get; set; }

        public virtual Order? Order { get; set; }
    }
}
