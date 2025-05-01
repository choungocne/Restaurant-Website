using System;
using System.Collections.Generic;

namespace RestaurantWebsite.Models;

public partial class DiningTable
{
    public int TableId { get; set; }

    public string? TableName { get; set; }

    public string? Location { get; set; }
    public string? Img {  get; set; }
    public int Quantity {  get; set; }
    public int NumberOfCustomer { get; set; }
    public bool IsAvailable { get; set; } = true;

    public virtual ICollection<OrderService> OrderServices { get; set; } = new List<OrderService>();
}
