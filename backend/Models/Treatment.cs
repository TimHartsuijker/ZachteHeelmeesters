using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Treatment
    {
        [Key]
        public int Id { get; set; }

        [StringLength(5)]
        public string Code { get; set; } = null!;

        [Required, MaxLength(255)]
        public string Description { get; set; } = null!;

        [Required]
        public int SpecialismId { get; set; }
        public Specialism Specialism { get; set; } = null!;

        [Required]
        public int DurationInQuarters { get; set; } // Per 15 minutes

        [Required]
        public decimal Cost { get; set; } // In euros
    }
}
