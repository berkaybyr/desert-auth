using System.ComponentModel.DataAnnotations;

namespace desert_auth.Models
{
    public class Register
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime RegisterDate { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "User Name")]
        public string Username { get; set; }
        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
        [MaxLength(32, ErrorMessage = "Password must be less than 32 characters long")]
        [DataType(DataType.Password)]

        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [Required]
        public string IP { get; set; }
    }
}
