using backend.Models;
using System.ComponentModel.DataAnnotations;

namespace backend.DTOs
{
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int RoleId { get; set; }
    }
}