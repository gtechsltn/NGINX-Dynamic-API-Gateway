using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Service2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        public ValuesController()
        {

        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Service 2 Message .");
        }
    }
}
