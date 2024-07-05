using System.ComponentModel.DataAnnotations;

namespace cube_gaming_store_back.DTOs.Posts
{
    public class SignInPostDTO
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        [MinLength(8)]
        public string? Password { get; set; }
    }
}