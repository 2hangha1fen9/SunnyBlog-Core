using CommentService.Request;
using CommentService.Response;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace CommentService.App.Interface
{
    public interface ILikeApp
    {
        Task<string> LikeArticle(int aid,int uid,int status);
        Task<PageList<LikeView>> GetUserLike(List<SearchCondition> condidtion, int uid, int status, int pageIndex, int pageSize);
        
    }
}
