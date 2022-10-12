using CommentService.Request;
using CommentService.Response;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace CommentService.App.Interface
{
    public interface ILikeApp
    {
        Task<string> LikeArticle(int aid,int uid,int status);
        Task<List<LikeView>> GetUserLike(int uid,bool isLike);
        Task<List<LikeView>> GetUserLikeMessage(int uid);
        Task<int> GetArticleLikeCount(int aid, int status);
        Task<string> DeleteLikeMessage(int uid, int lid, bool isAll = false);
    }
}
