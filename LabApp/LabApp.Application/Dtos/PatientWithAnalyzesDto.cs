namespace LabApp.Application.Dtos
{
    public class PatientWithAnalyzesDto
    {
        public int PatientId { get; set; }
        public string FullName { get; set; }
        public int AnalyzesCount { get; set; }
        public string Frequency { get; set; }
    }
}
