using System.ComponentModel.DataAnnotations;

namespace cube_gaming_store_back.DTOs.Posts
{
    public class RolePostDTO
    {
        [Required]
        public string? Role { get; set; }
    }
}