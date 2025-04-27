using System;
using System.Collections.Generic;

namespace RestaurantWebsite.Models;

public partial class Dish
{
    public int DishId { get; set; }

    public string DishName { get; set; } = null!;

    public decimal UnitPrice { get; set; }
    public string? Img { get; set; }
    public DateTime CreatedAt { get; set; }

    public int CategoryId { get; set; }

    public virtual DishCategory Category { get; set; } = null!;

    public virtual ICollection<OrderDish> OrderDishes { get; set; } = new List<OrderDish>();
}
