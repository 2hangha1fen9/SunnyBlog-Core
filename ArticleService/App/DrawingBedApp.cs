using ArticleService.App.Interface;
using ArticleService.Domain;
using ArticleService.Request;
using ArticleService.Response;
using Microsoft.EntityFrameworkCore;

namespace ArticleService.App
{
    public class DrawingBedApp : IDrawingBedApp
    {
        /// <summary>
        /// 数据库上下文
        /// </summary>
        private readonly IDbContextFactory<ArticleDBContext> contextFactory;

        public DrawingBedApp(IDbContextFactory<ArticleDBContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="request"></param>
        /// <param name="aid">文章ID如果为空则不会保存数据库记录</param>
        /// <param name="uid">用户ID</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<UploadResult> UploadPicture(UploadReq request, int? aid, int? uid)
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
                    if (aid.HasValue)
                    {
                        //将路径写入数据库
                        using (var dbContext = contextFactory.CreateDbContext())
                        {
                            //保存数据
                            Article? article = await dbContext.Articles.FirstOrDefaultAsync(a => a.Id == aid && a.UserId == uid);
                            if (article != null)
                            {
                                article.Photo = $"/picture/{fileName}";
                                await dbContext.SaveChangesAsync();
                            }
                        }
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
