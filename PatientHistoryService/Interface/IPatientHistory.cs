using Microsoft.AspNetCore.Mvc;
using PatientHistoryService.DTO.RequestDTO;

namespace PatientHistoryService.Interface
{
    public interface IPatientHistory
    {
        public Task<ActionResult<List<object>>> AddPatientHistory(HistoryRequest historyRequestDto, int userId);
        public Task<ActionResult<List<object>>> GetHistory(int PatientId, int userId);


    }
}
