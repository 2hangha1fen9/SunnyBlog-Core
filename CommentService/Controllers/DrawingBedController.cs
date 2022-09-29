using CommentService.App.Interface;
using CommentService.Request;
using CommentService.Response;
using Infrastructure;
using Infrastructure.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CommentService.Controllers
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
        public async Task<Response<UploadResult>> Upload([FromForm] UploadReq request)
        {
            var result = new Response<UploadResult>();
            try
            {
                result.Result = await drawingBedApp.UploadPicture(request);
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
