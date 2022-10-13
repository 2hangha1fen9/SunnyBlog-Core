using CommentService.Response;
namespace CommentService.App.Interface
{
    /// <summary>
    /// 文章数据统计
    /// </summary>
    public interface IMetaApp
    {
        Meta GetMeta(int aid, int? uid = null);
        List<Meta> GetMetaList(int[] aids, int? uid = null);
        Task<UserMeta> GetUserMeta(int uid);
    }
}
