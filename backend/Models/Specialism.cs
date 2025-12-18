using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Specialism
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;

        public ICollection<Treatment> Treatments { get; set; } = []; // NP for treatments under this specialism

        public ICollection<User> Specialists { get; set; } = []; // NP for specialists in this specialism
    }
}
