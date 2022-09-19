using CommentService.App.Interface;
using CommentService.Rpc.Protos;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using static ArticleService.Rpc.Protos.gArticle;
using static CommentService.Rpc.Protos.gUser;

namespace CommentService.Rpc.Service
{
    public class GRankService : gRank.gRankBase
    {
        private readonly gArticleClient aritlceRpc;
        private readonly IMetaApp metaApp;

        public GRankService(gArticleClient aritlceRpc, IMetaApp metaApp)
        {
            this.aritlceRpc = aritlceRpc;
            this.metaApp = metaApp;
        }

        /// <summary>
        /// 获取文章排行榜
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<RankList> GetArticleRank(RankCondidtion request, ServerCallContext context)
        {
            //获取所有文章id
            var articleIds = (await aritlceRpc.GetArticleListAsync(new Empty())).Infos.Select(a => a.Id).ToArray();
            //获取所有文章元数据
            var metaList = await metaApp.GetMetaList(articleIds);
            //针对不同的排序条件进行排序
            if ("ViewCount".Equals(request.Key, StringComparison.OrdinalIgnoreCase))
            {
                if(request.Order == -1)
                {
                    metaList =  metaList.OrderByDescending(m => m.ViewCount).ToList();
                }
                else
                {
                    metaList = metaList.OrderBy(m => m.ViewCount).ToList();
                }
            }
            else if("LikeCount".Equals(request.Key, StringComparison.OrdinalIgnoreCase))
            {
                if (request.Order == -1)
                {
                    metaList = metaList.OrderByDescending(m => m.LikeCount).ToList();
                }
                else
                {
                    metaList = metaList.OrderBy(m => m.LikeCount).ToList();
                }
            }
            else if ("CommentCount".Equals(request.Key, StringComparison.OrdinalIgnoreCase))
            {
                if (request.Order == -1)
                {
                    metaList = metaList.OrderByDescending(m => m.CommentCount).ToList();
                }
                else
                {
                    metaList = metaList.OrderBy(m => m.CommentCount).ToList();
                }
            }
            else if ("CollectionCount".Equals(request.Key, StringComparison.OrdinalIgnoreCase))
            {
                if (request.Order == -1)
                {
                    metaList = metaList.OrderByDescending(m => m.CollectionCount).ToList();
                }
                else
                {
                    metaList = metaList.OrderBy(m => m.CollectionCount).ToList();
                }
            }
            //仅返回id序列
            var rankList = new RankList();
            rankList.Ranks.AddRange(metaList.Select(m => m.ArticleId));
            return rankList;
        }
    }
}
