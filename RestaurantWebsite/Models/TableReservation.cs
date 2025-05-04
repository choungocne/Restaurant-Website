using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantWebsite.Models;

public partial class TableReservation
{
    public int ReservationId { get; set; }

    public int CustomerId { get; set; }
    public int? ServiceId { get; set; }

    public int TableId { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn thời gian bắt đầu")]
    public DateTime StartTime { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn thời gian kết thúc")]
    public DateTime EndTime { get; set; }

    public int? EmployeeId { get; set; } // Selected chef for this reservation

    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Navigation properties
    public virtual Customer? Customer { get; set; } = null!;
    public virtual DiningTable? Table { get; set; } = null!;
    public virtual Employee? Employee { get; set; }
    [ForeignKey("ServiceId")]
    public virtual OrderService? OrderService { get; set; }
}

public enum ReservationStatus
{
    Pending = 0,
    Confirmed = 1,
    Completed=2,
    Cancelled = 3
}