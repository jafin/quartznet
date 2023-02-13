using Microsoft.AspNetCore.Mvc;

using QuartzDemo.AppRestart;

namespace Quartz.AppRestart.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AppController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            Program.cancelTokenSource.Cancel();
            return "OK";
        }
    }
}