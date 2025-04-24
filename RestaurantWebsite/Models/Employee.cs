using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace RestaurantWebsite.Models
{
    public class Employee
    {
        public int EmployeeID { get; set; }
        [Required, StringLength(255)]
        public string FullName { get; set; }
        [Required]
        public DateTime BirthDate { get; set; }
        [Required, StringLength(15)]
        public string PhoneNummber { get; set; }
    }
}
