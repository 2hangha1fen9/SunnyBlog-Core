using CommentService.Response;

namespace CommentService.App.Interface
{
    public interface IStatisticsApp
    {
        Task<CommentCountStatistics> GetCommentCount(int? uid = null);
    }
}
