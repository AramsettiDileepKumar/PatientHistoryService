using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PatientHistoryService.DTO.RequestDTO;
using PatientHistoryService.Interface;
using System.Security.Claims;

namespace PatientHistoryService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientHistoryController : ControllerBase
    {
        private readonly IPatientHistory patientHistory;
        public PatientHistoryController(IPatientHistory patientHistory)
        {
            this.patientHistory = patientHistory;
        }
        [Authorize(Roles = "Doctor")]
        [HttpPost("AddHistory")]
        public async Task<IActionResult> AddPatientHistory(HistoryRequest historyRequestDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var result = await patientHistory.AddPatientHistory(historyRequestDto, userId);
                if (result != null)
                {
                    return Ok(new { success = true, Message = "Patient History Added", Data = result });
                }
                return BadRequest(new { success = false, Message = "SomeThing Went Wrong" });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Message = $"An Error Occured While Adding Patient History{ex.Message}" });
            }
        }
        [Authorize(Roles = "Doctor")]
        [HttpGet]
        public async Task<ActionResult<List<object>>> GetHistory(int PatientId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var result = await patientHistory.GetHistory(PatientId, userId);
                if (result != null)
                {
                    return Ok(new { success = true, Message = "Patient History Fetched", Data = result });
                }
                return BadRequest(new { success = false, Message = "SomeThing Went Wrong" });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Message = $"An Error Occured While Fetching Patient Details{ex.Message}" });
            }

        }
    }
}
