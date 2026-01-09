using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string RoleName { get; set; } = null!;

        public ICollection<User> Users { get; set; } = []; // NP for users with this role
    }
}
