using CommentService.Request;
using CommentService.Response;

namespace CommentService.App.Interface
{
    public interface IDrawingBedApp
    {
        Task<UploadResult> UploadPicture(UploadReq request);
    }
}
