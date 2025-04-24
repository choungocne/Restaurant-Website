using System;
using System.Collections.Generic;

namespace RestaurantWebsite.Models;

public partial class OrderService
{
    public int ServiceId { get; set; }

    public int TableId { get; set; }

    public int CustomerId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<OrderDish> OrderDishes { get; set; } = new List<OrderDish>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual DiningTable Table { get; set; } = null!;
}
