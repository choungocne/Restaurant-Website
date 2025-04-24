using System;
using System.Collections.Generic;

namespace RestaurantWebsite.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int ServiceId { get; set; }

    public int EmployeeId { get; set; }

    public decimal? TotalAmount { get; set; }

    public decimal? Discount { get; set; }

    public DateTime? PaymentTime { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual OrderService Service { get; set; } = null!;
}
