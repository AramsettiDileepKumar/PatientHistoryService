namespace PatientHistoryService.DTO.RequestDTO
{
    public class HistoryRequest
    {
        public int PatientId { get; set; }
        public string Issue { get; set; }
        public DateTime VisitsToDoctor { get; set; }
    }
}
