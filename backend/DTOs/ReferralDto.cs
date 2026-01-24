namespace backend.DTOs
{
    public class ReferralDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public int TreatmentId { get; set; }
        public string TreatmentCode { get; set; } = null!;
        public string TreatmentDescription { get; set; } = null!;
        public int PatientId { get; set; }
        public string PatientName { get; set; } = null!;
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = null!;
        public string Note { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
