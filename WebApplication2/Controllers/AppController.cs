using Microsoft.AspNetCore.Mvc;

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