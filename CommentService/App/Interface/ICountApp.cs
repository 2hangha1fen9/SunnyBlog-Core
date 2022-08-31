using CommentService.Response;
namespace CommentService.App.Interface
{
    /// <summary>
    /// 文章数据统计
    /// </summary>
    public interface ICountApp
    {
        Task<ArticleCountView> GetArticleCount(int aid,int? uid = null);
    }
}
