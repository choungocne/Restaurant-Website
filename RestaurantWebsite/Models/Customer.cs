using System;
using System.Collections.Generic;

namespace RestaurantWebsite.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string CustomerName { get; set; } = null!;

    public string? Address { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Img { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<OrderService> OrderServices { get; set; } = new List<OrderService>();
    public virtual ICollection<UserAccount> UserAccounts { get; set; } = new List<UserAccount>();
    public virtual ICollection<TableReservation> TableReservations { get; set; } = new List<TableReservation>();


}
