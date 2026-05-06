namespace LabApp.Domain.Entities
{
    public class PatientInsurance
    {
        public int PatientId { get; set; }
        public int InsuranceId { get; set; }

        // Навигационные свойства
        public virtual Patient Patient { get; set; }
        public virtual InsuranceCompany Insurance { get; set; }
    }
}
