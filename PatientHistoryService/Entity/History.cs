namespace PatientHistoryService.Entity
{
    public class History
    {
        public int HistoryId { get; set; }
        public int DoctorId { get; set; }
        public int PatentId { get; set; }
        public string Issue { get; set; }
        public DateTime VisitsToDoctor { get; set; }

    }
}
