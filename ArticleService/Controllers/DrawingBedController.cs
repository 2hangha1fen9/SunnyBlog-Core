using ArticleService.App.Interface;
using ArticleService.Request;
using ArticleService.Response;
using Infrastructure;
using Infrastructure.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ArticleService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DrawingBedController : ControllerBase
    {
        private readonly IDrawingBedApp drawingBedApp;

        public DrawingBedController(IDrawingBedApp drawingBedApp)
        {
            this.drawingBedApp = drawingBedApp;
        }

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [RBAC]
        [HttpPut]
        public async Task<Response<UploadResult>> Upload([FromForm] UploadReq request, [FromQuery] int? aid, [FromQuery] int? uid = null)
        {
            var result = new Response<UploadResult>();
            try
            {
                //获取token中的用户ID
                var userId = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "user_id")?.Value);

                if (uid.HasValue)
                {
                    userId = uid.Value;
                }

                result.Result = await drawingBedApp.UploadPicture(request, aid,userId);
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }
    }
}
