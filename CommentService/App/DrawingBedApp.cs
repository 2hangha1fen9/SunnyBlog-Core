using CommentService.App.Interface;
using CommentService.Domain;
using CommentService.Request;
using CommentService.Response;
using Microsoft.EntityFrameworkCore;

namespace CommentService.App
{
    public class DrawingBedApp : IDrawingBedApp
    {
        public DrawingBedApp() 
        { 
        
        }

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<UploadResult> UploadPicture(UploadReq request)
        {
            try
            {
                var fileExtension = Path.GetExtension(request.Data.FileName);
                string fileName = $"{DateTime.Now.ToFileTime()}{fileExtension}";
                string phyPath = Path.Combine(Directory.GetCurrentDirectory(), "static", "picture", fileName);
                if (!(fileExtension != ".png" || fileExtension != ".jpeg" || fileExtension != ".gif" || fileExtension != ".bmp" || fileExtension != ".ico"))
                {
                    throw new Exception("图片格式错误：只能为：png、jpeg、gif、bmp、ico");
                }
                //获取物理路径
                if (request.Data.Length > 0)
                {
                    using (var stream = File.Create(phyPath))
                    {
                        await request.Data.CopyToAsync(stream);
                    }
                    return new UploadResult()
                    {
                        Path = $"/picture/{fileName}"
                    };
                }
                throw new Exception("没有数据");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException?.Message ?? ex.Message);
            }
        }
    }
}
