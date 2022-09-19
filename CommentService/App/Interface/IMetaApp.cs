using CommentService.Response;
namespace CommentService.App.Interface
{
    /// <summary>
    /// 文章数据统计
    /// </summary>
    public interface IMetaApp
    {
        Task<Meta> GetMeta(int aid, int? uid = null);
        Task<List<Meta>> GetMetaList(int[] aids, int? uid = null);
    }
}
