using ArticleService.Rpc.Protos;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace ArticleService.Rpc.Service
{
    public class GArticleService : gArticle.gArticleBase
    {
        private readonly IDbContextFactory<ArticleDBContext> contextFactory;

        public GArticleService(IDbContextFactory<ArticleDBContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        /// <summary>
        /// 获取文章状态
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<ArticleInfo> GetArticleInfo(ArticleId request, ServerCallContext context)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var article = await dbContext.Articles.FirstOrDefaultAsync(a => a.Id == request.Id);
                if (article != null)
                {
                    var articleInfo = article.MapTo<ArticleInfo>();
                    return articleInfo;
                }
                return new ArticleInfo();
            }
        }

        public async override Task<ArticleInfoList> GetArticleList(Empty request, ServerCallContext context)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var articles = await dbContext.Articles.ToListAsync();
                if (articles != null)
                {
                    var articleInfo = new ArticleInfoList();
                    articleInfo.Infos.AddRange(articles.MapToList<ArticleInfo>());
                    return articleInfo;
                }
                return new ArticleInfoList();
            }
        }
    }
}
