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

        // GET api/record/6_6_2016/7_7_2017
        [HttpGet("{start}/{end}")]
        public JsonResult Get(string start, string end)
        {
            var result = RecordManager.GetRecords(start, end);
            return new JsonResult(result);
        }

    }

}