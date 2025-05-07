using System.ComponentModel.DataAnnotations;

namespace RestaurantWebsite.ViewModels
{
    public class EditEmployeeViewModel
    {
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [StringLength(255, ErrorMessage = "Họ tên không được vượt quá 255 ký tự")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng chọn ngày sinh")]
        [DataType(DataType.Date)]
        public DateOnly BirthDate { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? PhoneNumber { get; set; }

        public string? Detail { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string? Email { get; set; }

        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6-100 ký tự")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không trùng khớp")]
        public string? ConfirmPassword { get; set; }
        public string Role { get; set; }  // Thêm dòng này

    }
}
