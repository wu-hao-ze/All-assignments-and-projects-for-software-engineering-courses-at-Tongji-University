using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MeeTeam_Back;
using Oracle.ManagedDataAccess.Client;
using Microsoft.AspNetCore.Cors;
using SqlSugar;
using System.Data;
using System.Collections;
using MeeTeam_Backend;

namespace Meeteam_Backend.Controllers
{
    [Route("/[Controller]/[action]")]
    [ApiController]
    [EnableCors("any")]
    public class UserController : ControllerBase
    {

    }
}
