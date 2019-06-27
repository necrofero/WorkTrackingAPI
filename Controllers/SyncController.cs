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
    public class SyncController : ControllerBase
    {

        // GET api/sync/2019/25/456
        [HttpGet("{year}/{week}/{employee}")]
        public JsonResult Get(int year, int week, string employee)
        {
            var result = RecordManager.GetSyncedData(year, week, employee);
            return new JsonResult(result);
        }

        //POST api/sync/2019/25/456
        [HttpPost("{year}/{week}/{employee}")]
        public async Task<IActionResult> Post(int year, int week, string employee)
        {
            var result = await RecordManager.SetSyncedDataAsync(year, week, employee);
            return Ok(new JsonResult(result));
        }

    }

}