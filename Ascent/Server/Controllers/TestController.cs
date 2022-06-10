using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ascent.Server.Controllers
{
    [Authorize]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet("claims")]
        public object Index()
        {
            return User.Claims.Select(c => new { c.Type, c.Value }).ToList();
        }
    }
}
