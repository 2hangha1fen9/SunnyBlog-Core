using ArticleService.Request;
using ArticleService.Response;

namespace ArticleService.App.Interface
{
    public interface IDrawingBedApp
    {
        Task<UploadResult> UploadPicture(UploadReq request, int? aid,int? uid);
    }
}
