using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MessManagement.Shared.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }

        [StringLength(100)]
        public string FullName { get; set; } = null!;

        [StringLength(255)]
        public string Email { get; set; } = null!;

        [StringLength(255)]
        public string? GoogleId { get; set; }
        public byte[]? ProfilePicture { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? CurrentMessId { get; set; }
    }
}
