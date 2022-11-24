using DatingApp.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(LogUserLastActive))]
    public class BaseApiController : ControllerBase
    {
    }
}
