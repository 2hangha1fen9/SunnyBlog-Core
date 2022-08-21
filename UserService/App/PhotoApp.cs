
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
                //获取图片数据的头部信息
                string[] data = request.Data.Split(","); 
                //获取图片Base64编码
                byte[] base64 = Convert.FromBase64String(data[1].Replace(" ", "+"));
                //判断图片格式
                var type = data[0].Replace("data:","").Replace(";base64","").Split("/");
                if(type[0] != "image" && (type[1] != "png" || type[1] != "jpeg" || type[1] != "gif" || type[1] != "bmp" || type[1] != "ico")){
                    throw new Exception("图片格式错误：只能为：png、jpeg、gif、bmp、ico");
                }
                //获取物理路径
                string phyPath = Path.Combine(Directory.GetCurrentDirectory(), "static", "avatar", $"{id}.{type[1]}");
                //将base64转为图片
                await File.WriteAllBytesAsync(phyPath, base64);
                //将路径写入数据库
                using (var dbContext = contextFactory.CreateDbContext())
                {
                    //保存数据
                    var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);
                    if (user != null)
                    {
                        user.Photo = $"/api/avatar/{id}.png";
                    }
                    else
                    {
                        throw new Exception("用户信息错误");
                    }
                    if (await dbContext.SaveChangesAsync() > 0)
                    {
                        return new UploadResult()
                        {
                            Path = $"/api/avatar/{id}.{type[1]}"
                        };
                    }
                    else
                    {
                        throw new Exception("用户信息保存错误");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException?.Message ?? ex.Message);
            }
        }
    }
}
