
using Infrastructure;
using UserService.Request;
using UserService.Response;

namespace UserService.App.Interface
{
    public interface IPhotoApp
    {
        Task<UploadResult> UploadPhoto(UploadPhotoReq request, int id, string type);
    }
}
