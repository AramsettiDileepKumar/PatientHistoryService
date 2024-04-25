using Dapper;
using Microsoft.AspNetCore.Mvc;
using PatientHistoryService.DapperContext;
using PatientHistoryService.DTO.RequestDTO;
using PatientHistoryService.DTO.ResponseDTO;
using PatientHistoryService.Entity;
using PatientHistoryService.Interface;
using System.Data.SqlClient;

namespace PatientHistoryService.Service
{
    public class PatientHistoryServices:IPatientHistory
    {
        private readonly PatientHistoryContext _context;
        private readonly IHttpClientFactory httpClientFactory;

        public PatientHistoryServices(PatientHistoryContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<ActionResult<List<object>>> AddPatientHistory(HistoryRequest historyRequestDto, int doctorId)
        {
            try
            {
                string HistoryQuery = "INSERT INTO History(PatientId,DoctorId, Issue, VisitsToDoctor) VALUES(@PatientId,@DoctorId, @Issue , @VisitsToDoctor); ";
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("PatientId", historyRequestDto.PatientId);
                dynamicParameters.Add("DoctorId", doctorId);
                dynamicParameters.Add("Issue", historyRequestDto.Issue);
                dynamicParameters.Add("VisitsToDoctor", historyRequestDto.VisitsToDoctor);
                User user = getUserById(historyRequestDto.PatientId);
                var query = @"INSERT INTO PatientHistory (PatientId, PatientName,Email) VALUES (@PatientId, @PatientName,@Email)";
                DynamicParameters dynamicParameter = new DynamicParameters();
                dynamicParameter.Add("PatientId", user.UserId);
                dynamicParameter.Add("PatientName", user.FirstName);
                dynamicParameter.Add("Email", user.Email);
                using (var connection = _context.CreateConnection())
                {
                    await connection.ExecuteAsync(HistoryQuery, dynamicParameters);
                    var val = await GetPatientDetails(user.UserId);
                    if (val == null)
                    {
                        await connection.ExecuteAsync(query, dynamicParameter);
                    }
                    return await GetHistory(user.UserId, doctorId);
                }

            }
            catch(SqlException ex)
            {
              throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public User getUserById(int patientId)
        {
            try
            {
                var httpclient = httpClientFactory.CreateClient("userByid");
                var response = httpclient.GetAsync($"GetUserById{patientId}").Result;
                if (response.IsSuccessStatusCode)
                {
                    return response.Content.ReadFromJsonAsync<User>().Result;
                }
                throw new Exception("UserNotFound");
            }
            catch(Exception ex) 
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ActionResult<List<object>>> GetHistory(int patientId, int doctorId)
        {
            try
            {
                PatientHistory patientHistory = await GetPatientDetails(patientId);
                List<HistoryResponse> patientHistoryList = await GetPatientHistory(patientId, doctorId);
                var result = new List<object> { patientHistory, patientHistoryList };
                return result;
            }
            catch (Exception ex)
            {
                return new List<object> { new { Error = $"An error occurred: {ex.Message}" } };
            }
        }


        private async Task<PatientHistory> GetPatientDetails(int patientId)
        {
            string selectPatientQuery = @"SELECT PatientId, PatientName, Email FROM PatientHistory WHERE PatientId = @PatientId;";

            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<PatientHistory>(selectPatientQuery, new { PatientId = patientId });
            }
        }

        private async Task<List<HistoryResponse>> GetPatientHistory(int patientId, int doctorId)
        {
            string selectHistoryQuery = @"SELECT Issue, VisitsToDoctor FROM History WHERE PatientId = @PatientId AND DoctorId = @DoctorId;";

            using (var connection = _context.CreateConnection())
            {
                return (await connection.QueryAsync<HistoryResponse>(selectHistoryQuery, new { PatientId = patientId, DoctorId = doctorId })).ToList();
            }
        }
    }
}
