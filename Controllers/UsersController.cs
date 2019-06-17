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
    public class UserController : ControllerBase
    {

        [Route("authenticate")]
        [HttpPost]
        public JsonResult Authenticate([FromBody] UserLogin user)
        {
            var result = UserManager.Authenticate(user);
            return new JsonResult(result);
        }

    }

}