using API_Classes;
using DataWebAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DataWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValuesController : ControllerBase
    {
        private readonly List<DataIntermed> _data = DataServer.Instance.Data;

        [HttpGet]
        public ActionResult<int> GetCount() => Ok(_data.Count);

        [HttpGet("{index}")]
        public ActionResult<DataIntermed> GetByIndex(int index)
        {
            if (index < 0 || index >= _data.Count)
                return BadRequest($"Index must be between 0 and {_data.Count - 1}");
            return Ok(_data[index]);
        }

        [HttpPost("search")]
        public ActionResult<DataIntermed> Search([FromBody] SearchData query)
        {
            if (query == null || string.IsNullOrWhiteSpace(query.searchStr))
                return BadRequest("searchStr required in body");

            var r = _data.Find(d =>
                d.fname.Contains(query.searchStr, System.StringComparison.OrdinalIgnoreCase) ||
                d.lname.Contains(query.searchStr, System.StringComparison.OrdinalIgnoreCase)
            );

            if (r == null) return NotFound("No matching record found");
            return Ok(r);
        }
    }
}
