using ArticleService.Rpc.Protos;
using CommentService.App.Interface;
using CommentService.Domain;
using CommentService.Request;
using CommentService.Response;
using Google.Protobuf.WellKnownTypes;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using static ArticleService.Rpc.Protos.gArticle;
using static CommentService.Rpc.Protos.gUser;

namespace CommentService.App
{
    public class CommentApp : ICommentApp
    {
        private readonly IDbContextFactory<CommentDBContext> contextFactory;
        private readonly gArticleClient articleRpc;
        private readonly gUserClient userRpc;

        public CommentApp(IDbContextFactory<CommentDBContext> contextFactory, gArticleClient articleRpc, gUserClient userRpc)
        {
            this.contextFactory = contextFactory;
            this.articleRpc = articleRpc;
            this.userRpc = userRpc;
        }

        /// <summary>
        /// 评论文章
        /// </summary>
        /// <param name="request">评论内容</param>
        /// <param name="uid">用户ID</param>
        /// <returns></returns>
        public async Task<string> CommentArticle(CommentReq request,int uid)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                //获取文章状态,判断文章是否可以评论
                var articleInfo = await articleRpc.GetArticleInfoAsync(new ArticleId() { Id = request.ArticleId });
                var comment = request.MapTo<Comment>();
                comment.UserId = uid;
                comment.CreateTime = DateTime.Now;
                comment.Status = articleInfo.CommentStatus;
                if (articleInfo.CommentStatus != -1 || articleInfo.Status == 1 || articleInfo.IsLock == 1)
                {
                    await dbContext.Comments.AddAsync(comment);
                    if (await dbContext.SaveChangesAsync() < 0)
                    {
                        throw new Exception("评论失败");
                    }
                    return articleInfo.CommentStatus == 1 ? "评论成功" : "评论成功,需要作者审核后展示";
                }
                throw new Exception("作者没有开通评论");
            }
        }

        /// <summary>
        /// 删除评论(评论者)
        /// </summary>
        /// <param name="cid"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> DeleteComment(int cid,int uid)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var comment = await dbContext.Comments.FirstOrDefaultAsync(c => c.Id == cid && c.UserId == uid);
                if (comment != null)
                {
                    dbContext.Comments.Remove(comment);
                    if (await dbContext.SaveChangesAsync() < 0)
                    {
                        throw new Exception("评论删除失败");
                    }
                    return "删除成功";
                }
                throw new Exception("评论删除失败");
            }
        }

        /// <summary>
        /// 删除评论(管理员)
        /// </summary>
        /// <param name="cid"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> DeleteComment(int cid)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var comment = await dbContext.Comments.FirstOrDefaultAsync(c => c.Id == cid);
                if (comment != null)
                {
                    dbContext.Comments.Remove(comment);
                    if (await dbContext.SaveChangesAsync() < 0)
                    {
                        throw new Exception("评论删除失败");
                    }
                    return "删除成功";
                }
                throw new Exception("评论删除失败");
            }
        }

        /// <summary>
        /// 删除评论(作者)
        /// </summary>
        /// <param name="cid"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public async Task<string> DeleteArticleComment(int cid, int uid)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                //获取用户所有文章
                var articleList = (await articleRpc.GetArticleListAsync(new Empty())).Infos.Where(a => a.UserId == uid);
                //查询是否有这条评论
                var comment = await dbContext.Comments.FirstOrDefaultAsync(c => c.Id == cid);
                if (comment != null)
                {
                    //查询这条评论是否是这个作者的
                    if(articleList.FirstOrDefault(a => a.Id == comment.ArticleId) != null)
                    {
                        dbContext.Comments.Remove(comment);
                        if (await dbContext.SaveChangesAsync() < 0)
                        {
                            throw new Exception("评论删除失败");
                        }
                        return "删除成功";
                    }
                }
                throw new Exception("评论删除失败");
            }
        }
        
        /// <summary>
        /// 获取文章评论
        /// </summary>
        /// <param name="aid">文章ID</param>
        /// <returns></returns>
        public async Task<PageList<CommentView>> GetArticleComment(int aid,List<SearchCondition> condidtion, int pageIndex, int pageSize)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var commentList = await dbContext.Comments.Where(c => c.ArticleId == aid && c.Status == 1).ToListAsync();
                if (commentList != null)
                {
                    //调用rpc获取用户列表
                    var userList = (await userRpc.GetUserListAsync(new Empty())).UserInfo.ToList();
                    var commentView = commentList.Join(userList,c => c.UserId,u => u.Id,(c,u) => new
                    {
                        Id = c.Id,
                        ArticleId = c.ArticleId,
                        UserId = u.Id,
                        Nick = u.Nick,
                        Photo = u.Photo,
                        Content = c.Content,
                        CreateTime = c.CreateTime,
                        ParentId = c.ParentId,
                        InverseParent = c.InverseParent.MapToList<CommentView>()
                    }).Where(c => c.ParentId == null).AsQueryable();

                    //条件筛选
                    if (condidtion.Count > 0)
                    {
                        foreach (var con in condidtion)
                        {
                            commentView = "Nick".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? commentView.Where(c => c.Nick.Contains(con.Value)): commentView;
                            commentView = "Content".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? commentView.Where(c => c.Nick.Contains(con.Value)) : commentView;
                        }
                    }
                    //分页
                    var commentPage = new PageList<CommentView>();
                    commentView = commentPage.Pagination(pageIndex, pageSize, commentView);
                    commentPage.Page = commentView.MapToList<CommentView>();
                    return commentPage;
                }
                return null;
            }
        }

        /// <summary>
        /// 获取所有文章评论列表
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<PageList<CommentListView>> GetCommentList(List<SearchCondition> condidtion, int pageIndex, int pageSize)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var comment = await dbContext.Comments.ToListAsync();
                if (comment != null)
                {
                    //调用rpc获取用户列表
                    var userList = (await userRpc.GetUserListAsync(new Empty())).UserInfo.ToList();
                    //嗲用rpc获取文章列表
                    var articleList = (await articleRpc.GetArticleListAsync(new Empty())).Infos.ToList();
                    var commentView = (from c in comment
                                      join u in userList on c.UserId equals u.Id
                                      join a in articleList on c.ArticleId equals a.Id
                                      select new
                                      {
                                          Id = c.Id,
                                          ArticleId = a.Id,
                                          ArticleTitle = a.Title,
                                          UserId = u.Id,
                                          Content = c.Content,
                                          Nick = u.Nick,
                                          Username = u.Username,
                                          Status = c.Status,
                                          CreateTime = c.CreateTime
                                      }).AsQueryable();
                    //条件筛选
                    if (condidtion.Count > 0)
                    {
                        foreach (var con in condidtion)
                        {
                            commentView = "Nick".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? commentView.Where(c => c.Nick.Contains(con.Value)) : commentView;
                            commentView = "Content".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? commentView.Where(c => c.Nick.Contains(con.Value)) : commentView;
                            commentView = "Username".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? commentView.Where(c => c.Nick.Contains(con.Value)) : commentView;
                        }
                    }
                    //分页
                    var commentPage = new PageList<CommentListView>();
                    commentView = commentPage.Pagination(pageIndex, pageSize, commentView);
                    commentPage.Page = commentView.MapToList<CommentListView>();
                    return commentPage;
                }
                return null;
            }
        }

        /// <summary>
        /// 获取用户发表的评论
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public async Task<PageList<CommentListView>> GetUserCommentList(int uid, List<SearchCondition> condidtion, int pageIndex, int pageSize)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var comment = await dbContext.Comments.ToListAsync();
                if (comment != null)
                {
                    //调用rpc获取用户列表
                    var userList = (await userRpc.GetUserListAsync(new Empty())).UserInfo.ToList();
                    //调用rpc获取文章列表
                    var articleList = (await articleRpc.GetArticleListAsync(new Empty())).Infos.ToList();
                    var commentView = (from c in comment
                                       join u in userList on c.UserId equals u.Id
                                       join a in articleList on c.ArticleId equals a.Id
                                       where u.Id == uid
                                       select new
                                       {
                                           Id = c.Id,
                                           ArticleId = a.Id,
                                           ArticleTitle = a.Title,
                                           UserId = u.Id,
                                           Content = c.Content,
                                           Nick = u.Nick,
                                           Username = u.Username,
                                           Status = c.Status,
                                           CreateTime = c.CreateTime
                                       }).AsQueryable();

                    //条件筛选
                    if (condidtion.Count > 0)
                    {
                        foreach (var con in condidtion)
                        {
                            commentView = "Nick".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? commentView.Where(c => c.Nick.Contains(con.Value)) : commentView;
                            commentView = "Content".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? commentView.Where(c => c.Nick.Contains(con.Value)) : commentView;
                        }
                    }
                    //分页
                    var commentPage = new PageList<CommentListView>();
                    commentView = commentPage.Pagination(pageIndex, pageSize, commentView);
                    commentPage.Page = commentView.MapToList<CommentListView>();
                    return commentPage;
                }
                return null;
            }
        }

        /// <summary>
        /// 获取用户给我的的评论
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public async Task<PageList<CommentListView>> GetMyCommentList(int uid, List<SearchCondition> condidtion, int pageIndex, int pageSize)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var comment = await dbContext.Comments.ToListAsync();
                if (comment != null)
                {
                    //调用rpc获取用户列表
                    var userList = (await userRpc.GetUserListAsync(new Empty())).UserInfo.ToList();
                    //调用rpc获取文章列表
                    var articleList = (await articleRpc.GetArticleListAsync(new Empty())).Infos.Where(a => a.UserId == uid).ToList();
                    var commentView = (from c in comment
                                       join u in userList on c.UserId equals u.Id
                                       join a in articleList on c.ArticleId equals a.Id
                                       select new
                                       {
                                           Id = c.Id,
                                           ArticleId = a.Id,
                                           ArticleTitle = a.Title,
                                           UserId = u.Id,
                                           Content = c.Content,
                                           Nick = u.Nick,
                                           Username = u.Username,
                                           Status = c.Status,
                                           CreateTime = c.CreateTime
                                       }).AsQueryable();

                    //条件筛选
                    if (condidtion.Count > 0)
                    {
                        foreach (var con in condidtion)
                        {
                            commentView = "Nick".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? commentView.Where(c => c.Nick.Contains(con.Value)) : commentView;
                            commentView = "Content".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? commentView.Where(c => c.Nick.Contains(con.Value)) : commentView;
                        }
                    }
                    //分页
                    var commentPage = new PageList<CommentListView>();
                    commentView = commentPage.Pagination(pageIndex, pageSize, commentView);
                    commentPage.Page = commentView.MapToList<CommentListView>();
                    return commentPage;
                }
                return null;
            }
        }

        /// <summary>
        /// 已读评论
        /// </summary>
        /// <param name="cid"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<string> ReadComment(int cid,int uid)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var comment = await dbContext.Comments.Where(c => c.Id == cid && c.UserId == uid).FirstOrDefaultAsync();
                if (comment != null)
                {
                    comment.IsRead = 1;
                    dbContext.Comments.Update(comment);
                    if (await dbContext.SaveChangesAsync() < 0)
                    {
                        throw new Exception("操作失败");
                    }
                    return "操作成功";
                }
                throw new Exception("找不到此条评论");
            }
        }

        /// <summary>
        /// 审核评论
        /// </summary>
        /// <param name="cid"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<string> AllowComment(int cid, int uid)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var comment = await dbContext.Comments.Where(c => c.Id == cid && c.UserId == uid).FirstOrDefaultAsync();
                if (comment != null)
                {
                    comment.Status = 1;
                    comment.IsRead = 1;
                    dbContext.Comments.Update(comment);
                    if (await dbContext.SaveChangesAsync() < 0)
                    {
                        throw new Exception("操作失败");
                    }
                    return "操作成功";
                }
                throw new Exception("找不到此条评论");
            }
        }

        /// <summary>
        /// 获取文章评论数
        /// </summary>
        /// <param name="aid"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<int> GetArticleCommentCount(int aid)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var count = await dbContext.Comments.CountAsync(c => c.ArticleId == aid);
                return count;
            }
        }
    }
}
