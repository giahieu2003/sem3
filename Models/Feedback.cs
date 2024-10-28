using System;
using System.Collections.Generic;

namespace Sem3.Models
{
    public partial class Feedback
    {
        public int FeedbackId { get; set; }
        public int? CustomerId { get; set; }
        public string? FeedbackText { get; set; }
        public DateTime? SubmissionDate { get; set; }

        public virtual Customer? Customer { get; set; }
    }
}
