using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;

namespace Meeteam_Backend.Controllers
{
    [Route("/[Controller]/[action]")]
    [ApiController]
    [EnableCors("any")]
    public class LogoutController : ControllerBase
    {
        [HttpGet]
        public bool logout()
        {
            try 
            {
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }
    }
}
