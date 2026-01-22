using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class MedicalRecordFile
    {
        public int Id { get; set; }

        [Required]
        public int MedicalRecordEntryId { get; set; }
        public MedicalRecordEntry MedicalRecordEntry { get; set; } = null!;

        [Required]
        public string FileName { get; set; } = null!;

        [Required]
        public byte[] FileContent { get; set; } = null!;

        [Required]
        public string ContentType { get; set; } = null!;

        public long FileSize { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public string? Category { get; set; }

        public string? Description { get; set; }
    }
}