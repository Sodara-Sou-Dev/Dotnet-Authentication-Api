using System.ComponentModel.DataAnnotations;

namespace cube_gaming_store_back.DTOs.Posts
{
    public class SignUpPostDTO
    {
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        public string? Role { get; set; }
        [Required]
        [MinLength(8)]
        public string? Password { get; set; }
        [Required]
        [MinLength(8)]
        public string? ConfirmPassword { get; set; }
    }
}