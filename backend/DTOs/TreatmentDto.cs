using System.ComponentModel.DataAnnotations;

namespace backend.DTOs
{
    public class TreatmentDto
    {
        public int Id { get; set; }

        [Required]
        public string Code { get; set; } = null!;

        [Required]
        public string Description { get; set; } = null!;
    }
}
