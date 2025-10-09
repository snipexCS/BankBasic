using Microsoft.AspNetCore.Mvc;
using API_Classes;
using RestSharp;
using Newtonsoft.Json;

namespace BusinessWebAPI.Controllers
{
    [ApiController]
    [Route("api/business/[controller]")]
    public class ValuesController : ControllerBase
    {
        private readonly string _dataBaseUrl = "https://localhost:7294/api/values";

        [HttpGet]
        public async Task<ActionResult<int>> GetCountAsync()
        {
            try
            {
                var client = new RestClient(_dataBaseUrl);
                var response = await client.ExecuteAsync(new RestRequest());
                if (!response.IsSuccessful)
                    return StatusCode((int)response.StatusCode, response.Content);

                int total = JsonConvert.DeserializeObject<int>(response.Content!);
                return Ok(total);
            }
            catch (Exception ex)
            {
                return StatusCode(500, JsonConvert.SerializeObject(new ApiError { Message = ex.Message, StackTrace = ex.StackTrace }));
            }
        }

        [HttpGet("{index}")]
        public async Task<ActionResult<DataIntermedDTO>> GetByIndexAsync(int index)
        {
            try
            {
                var client = new RestClient($"{_dataBaseUrl}/{index}");
                var response = await client.ExecuteAsync(new RestRequest());
                if (!response.IsSuccessful)
                    return StatusCode((int)response.StatusCode, response.Content);

                var data = JsonConvert.DeserializeObject<DataIntermed>(response.Content!);
                if (data == null) return Problem("Failed to parse DataWebAPI response");

                var dto = new DataIntermedDTO
                {
                    acct = data.acct,
                    bal = data.bal,
                    pin = data.pin,
                    fname = data.fname,
                    lname = data.lname,
                    imageBase64 = data.image != null ? Convert.ToBase64String(data.image) : null
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, JsonConvert.SerializeObject(new ApiError { Message = ex.Message, StackTrace = ex.StackTrace }));
            }
        }

        

    }
}
