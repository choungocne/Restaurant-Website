using System;
using System.Collections.Generic;

namespace RestaurantWebsite.Models;

public partial class Feedback
{
    public int FeedbackId { get; set; }

    public int? CustomerId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual Customer? Customer { get; set; }
}
