using System;
using System.Collections.Generic;

namespace RestaurantWebsite.Models;

public partial class DishCategory
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public virtual ICollection<Dish> Dishes { get; set; } = new List<Dish>();
}
