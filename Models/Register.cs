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
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        [MaxLength(24, ErrorMessage = "Password must be less than 24 characters long")]
        public string Username { get; set; }
        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        [MaxLength(32, ErrorMessage = "Password must be less than 32 characters long")]
        [DataType(DataType.Password)]

        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }


        [Required]
        public string IP { get; set; }
    }
}
