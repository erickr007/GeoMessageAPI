using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GeoMessageAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/Data")]
    public class DataController : Controller
    {
        [Route("")]
        [Route("/")]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        //[Route("{value}")]
        [Route("echotest/{value}")]
        public string EchoTest(string value)
        {
            return value;
        }

        public string NoRoute()
        {
            return "No Route";
        }

    }
}