using System;
using System.Collections.Generic;

namespace RestaurantWebsite.Models;

public partial class OrderDish
{
    public int ServiceId { get; set; }

    public int DishId { get; set; }

    public int? Quantity { get; set; }

    public decimal? UnitPrice { get; set; }

    public string? Note { get; set; }

    public virtual Dish Dish { get; set; } = null!;

    public virtual OrderService Service { get; set; } = null!;
}
