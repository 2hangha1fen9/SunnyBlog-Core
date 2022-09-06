
using Microsoft.EntityFrameworkCore;
using UserService.App.Interface;
using UserService.Request;
using UserService.Response;

namespace UserService.App
{
    public class PhotoApp : IPhotoApp
    {
        /// <summary>
        /// 数据库上下文
        /// </summary>
        private readonly IDbContextFactory<UserDBContext> contextFactory;

        public PhotoApp(IDbContextFactory<UserDBContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        /// <summary>
        /// 上传用户头像
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<UploadResult> UploadPhoto(UploadPhotoReq request,int id)
        {
            try
            {
                var fileExtension = Path.GetExtension(request.Data.FileName);
                string phyPath = Path.Combine(Directory.GetCurrentDirectory(), "static", "avatar", $"{id}{fileExtension}");
                if (!(fileExtension != ".png" || fileExtension != ".jpeg" || fileExtension != ".gif" || fileExtension != ".bmp" || fileExtension != ".ico")){
                    throw new Exception("图片格式错误：只能为：png、jpeg、gif、bmp、ico");
                }
                //获取物理路径
                if (request.Data.Length > 0)
                {
                    using (var stream = File.Create(phyPath))
                    {
                        await request.Data.CopyToAsync(stream);
                    }
                    //将路径写入数据库
                    using (var dbContext = contextFactory.CreateDbContext())
                    {
                        //保存数据
                        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);
                        if (user != null)
                        {
                            user.Photo = $"/avatar/{id}{fileExtension}";
                            await dbContext.SaveChangesAsync();
                            return new UploadResult()
                            {
                                Path = $"/avatar/{id}{fileExtension}"
                            };
                        }
                        else
                        {
                            throw new Exception("用户信息保存错误");
                        }
                    }
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
