using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ArticleService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        /// <summary>
        /// 健康检查地址 /HealthCheck
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = "GetHealthCheck")]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}
