using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WorkTrackingAPI.Managers;
using WorkTrackingAPI.Models;

namespace WorkTrackingAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class RecordController : ControllerBase
    {

        // GET api/record/6_6_2016/7_7_2017/456
        [HttpGet("{start}/{end}/{employee}")]
        public JsonResult Get(string start, string end, string employee)
        {
            var result = RecordManager.GetRecords(start, end, employee);
            return new JsonResult(result);
        }

        // POST api/record
        [HttpPost()]
        public JsonResult Post([FromBody] WorkRecord record)
        {
            var result = RecordManager.InsertRecord(record);
            return new JsonResult(result);
        }

        // DELETE api/record/5
        [HttpDelete("{recordId}")]
        public void Delete(string recordId)
        {
            RecordManager.DeleteRecord(recordId);
        }

    }

}