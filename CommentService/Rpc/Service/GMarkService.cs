using CommentService.App.Interface;
using CommentService.Rpc.Protos;
using Grpc.Core;

namespace CommentService.Rpc.Service
{
    public class GMarkService:gMark.gMarkBase
    {
        private readonly ILikeApp likeApp;

        public GMarkService(ILikeApp likeApp)
        {
            this.likeApp = likeApp;
        }

        //获取用户的点赞/收藏
        public async override Task<ArticleIdList> GetUserLike(RequestInfo request, ServerCallContext context)
        {
            var ids = new ArticleIdList();
            var likes = await likeApp.GetUserLike(request.Uid, request.Type == 1);
            if(likes.Count > 0)
            {
                ids.Ids.AddRange(likes.Select(x => x.ArticleId).ToArray());
            }
            return ids;
        }
    }
}
