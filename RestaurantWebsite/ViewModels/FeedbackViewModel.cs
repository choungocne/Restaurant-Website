using System.ComponentModel.DataAnnotations;

namespace RestaurantWebsite.ViewModels
    {
        public class FeedbackViewModel
        {
            public int ReservationId { get; set; }

            public int CustomerId { get; set; }

            public DateTime ReservationDate { get; set; }

            public string TableName { get; set; }

            [Required(ErrorMessage = "Vui lòng chọn đánh giá từ 1-5 sao")]
            [Range(1, 5, ErrorMessage = "Đánh giá phải từ 1 đến 5 sao")]
            public int Rating { get; set; }

            [Required(ErrorMessage = "Vui lòng nhập nội dung đánh giá")]
            [StringLength(500, ErrorMessage = "Nội dung đánh giá không được quá 500 ký tự")]
            public string Content { get; set; }
        }
    }

