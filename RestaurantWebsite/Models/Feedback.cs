using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RestaurantWebsite.Models;

public partial class Feedback
{
    public int FeedbackId { get; set; }

    public int? CustomerId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual Customer? Customer { get; set; }

    [Required]
    public int ReservationId { get; set; }

    [Required]
    [Range(1, 5)]
    public int Rating { get; set; }
    public virtual TableReservation? Reservation { get; set; }
    }



