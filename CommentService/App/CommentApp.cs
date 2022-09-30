using ArticleService.Rpc.Protos;
using CommentService.App.Interface;
using CommentService.Domain;
using CommentService.Request;
using CommentService.Response;
using CommentService.Rpc.Protos;
using Google.Protobuf.WellKnownTypes;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using static ArticleService.Rpc.Protos.gArticle;
using static ArticleService.Rpc.Protos.gSetting;
using static CommentService.Rpc.Protos.gUser;

namespace CommentService.App
{
    public class CommentApp : ICommentApp
    {
        private readonly IDbContextFactory<CommentDBContext> contextFactory;
        private readonly gArticleClient articleRpc;
        private readonly gUserClient userRpc;
        private readonly gSettingClient settingRpc;

        public CommentApp(IDbContextFactory<CommentDBContext> contextFactory, gArticleClient articleRpc, gUserClient userRpc, gSettingClient settingRpc)
        {
            this.contextFactory = contextFactory;
            this.articleRpc = articleRpc;
            this.userRpc = userRpc;
            this.settingRpc = settingRpc;
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
                var comment = request.MapTo<Comment>();
                //获取文章状态,判断文章是否可以评论
                var articleInfo = await articleRpc.GetArticleInfoAsync(new ArticleId() { Id = request.ArticleId });
                //获取全局设置
                var commentStatus = (await settingRpc.GetGlobalSettingAsync(new Empty())).Settings.FirstOrDefault(s => s.Option == "CommentStatus");
                if (commentStatus != null && commentStatus.Value != 1)
                {
                    articleInfo.CommentStatus = commentStatus.Value;
                    if(articleInfo.CommentStatus == -1)
                    {
                        return "作者没有开通评论";
                    }
                }
                comment.UserId = uid;
                comment.CreateTime = DateTime.Now;
                comment.Status = articleInfo.CommentStatus;
                if (articleInfo.CommentStatus != -1 && articleInfo.Status == 1 && articleInfo.IsLock == 1)
                {
                    await dbContext.Comments.AddAsync(comment);
                    if (await dbContext.SaveChangesAsync() < 0)
                    {
                        throw new Exception("评论失败");
                    }
                    return articleInfo.CommentStatus == 1 ? "评论成功" : "评论成功,需要作者审核后展示";
                }
                return "作者没有开通评论";
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
                var comments = await dbContext.Comments.ToListAsync(); //获取所有评论
                var comment = comments.FirstOrDefault(c => c.Id == cid && c.UserId == uid);
                if (comment != null)
                {
                    //取消对应的子评论关系
                    foreach (var child in comment.InverseParent)
                    {
                        child.ParentId = null;
                        dbContext.Entry(child).State = EntityState.Modified;
                    }
                    await dbContext.SaveChangesAsync();
                    dbContext.Comments.Remove(comment);
                    if (await dbContext.SaveChangesAsync() < 0)
                    {
                        throw new Exception("评论删除失败");
                    }
                    return "删除成功";
                }
                else
                {
                    throw new Exception("没有此评论");
                }
                
            }
        }

        /// <summary>
        /// 删除评论(管理员)
        /// </summary>
        /// <param name="cid"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> DeleteComment(int[] cids)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var comments = await dbContext.Comments.ToListAsync(); //获取所有评论
                foreach (var cid in cids)
                {
                    //查询将删除的评论
                    var comment = comments.FirstOrDefault(c => c.Id == cid);
                    if(comment != null)
                    {
                        //取消对应的子评论关系
                        foreach (var child in comment.InverseParent)
                        {
                            child.ParentId = null;
                            dbContext.Entry(child).State = EntityState.Modified;
                        }
                        await dbContext.SaveChangesAsync();
                    }
                    dbContext.Comments.Remove(comment);
                }

                if (await dbContext.SaveChangesAsync() < 0)
                {
                    throw new Exception("评论删除失败");
                }
                return "删除成功";
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
                //获取所有评论
                var comments = await dbContext.Comments.ToListAsync(); 
                //查询是否有这条评论
                var comment = comments.FirstOrDefault(c => c.Id == cid);
                if (comment != null)
                {
                    //查询这条评论是否是这个作者的
                    if(articleList.FirstOrDefault(a => a.Id == comment.ArticleId) != null)
                    {
                        //取消对应的子评论关系
                        foreach (var child in comment.InverseParent)
                        {
                            child.ParentId = null;
                            dbContext.Entry(child).State = EntityState.Modified;
                        }
                        await dbContext.SaveChangesAsync();
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
        public async Task<PageList<CommentView>> GetArticleComment(int aid,List<SearchCondition>? condidtion, int pageIndex, int pageSize)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var commentList = await dbContext.Comments.Where(c => c.ArticleId == aid && c.Status == 1).ToListAsync();
                if (commentList != null)
                {
                    //调用rpc获取用户列表
                    var userList = (await userRpc.GetUserListAsync(new Empty())).UserInfo.ToList();
                    var commentView = commentList.Where(c => c.ParentId == null).MapToList<CommentView>();

                    FillUserInfo(commentView, userList);
                    //条件筛选
                    if (condidtion?.Count > 0)
                    {
                        foreach (var con in condidtion)
                        {
                            commentView = "Nick".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? commentView.Where(c => c.Nick.Contains(con.Value)).ToList() : commentView;
                            commentView = "Content".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? commentView.Where(c => c.Content.Contains(con.Value)).ToList() : commentView;
                            commentView = "Username".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? commentView.Where(c => c.Username.Contains(con.Value)).ToList() : commentView;
                            //排序
                            if (con.Sort != 0)
                            {
                                if ("CreateTime".Equals(con.Key, StringComparison.OrdinalIgnoreCase))
                                {
                                    if (con.Sort == -1)
                                    {
                                        commentView = commentView.OrderByDescending(a => a.CreateTime).ToList();
                                    }
                                    else
                                    {
                                        commentView = commentView.OrderBy(a => a.CreateTime).ToList();
                                    }
                                }
                            }
                            else
                            {
                                //默认时间排序
                                commentView = commentView.OrderByDescending(a => a.CreateTime).ToList();
                            }
                        }
                    }
                    else
                    {
                        //默认时间排序
                        commentView = commentView.OrderByDescending(a => a.CreateTime).ToList();
                    }
                    //分页
                    var commentPage = new PageList<CommentView>();
                    commentPage.Page = commentPage.Pagination(pageIndex, pageSize, commentView.AsQueryable()).ToList();
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
        public async Task<PageList<CommentListView>> GetCommentList(List<SearchCondition>? condidtion, int pageIndex, int pageSize)
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
                    if (condidtion?.Count > 0)
                    {
                        foreach (var con in condidtion)
                        {
                            commentView = "Nick".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? commentView.Where(c => c.Nick.Contains(con.Value)) : commentView;
                            commentView = "Content".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? commentView.Where(c => c.Content.Contains(con.Value)) : commentView;
                            commentView = "ArticleTitle".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? commentView.Where(c => c.ArticleTitle.Contains(con.Value)) : commentView;
                            commentView = "Username".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? commentView.Where(c => c.Username.Contains(con.Value)) : commentView;
                            commentView = "Status".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? commentView.Where(c => c.Status == Convert.ToInt32(con.Value)) : commentView;
                            //排序
                            if (con.Sort != 0)
                            {
                                if ("CreateTime".Equals(con.Key, StringComparison.OrdinalIgnoreCase))
                                {
                                    if (con.Sort == -1)
                                    {
                                        commentView = commentView.OrderByDescending(a => a.CreateTime);
                                    }
                                    else
                                    {
                                        commentView = commentView.OrderBy(a => a.CreateTime);
                                    }
                                }
                            }
                            else
                            {
                                //默认时间排序
                                commentView = commentView.OrderByDescending(a => a.CreateTime);
                            }
                        }
                    }
                    else
                    {
                        //默认时间排序
                        commentView = commentView.OrderByDescending(a => a.CreateTime);
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
        public async Task<PageList<CommentListView>> GetUserCommentList(int uid, List<SearchCondition>? condidtion, int pageIndex, int pageSize)
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
                    if (condidtion?.Count > 0)
                    {
                        foreach (var con in condidtion)
                        {
                            commentView = "Nick".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? commentView.Where(c => c.Nick.Contains(con.Value)) : commentView;
                            commentView = "Content".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? commentView.Where(c => c.Content.Contains(con.Value)) : commentView;
                            commentView = "ArticleTitle".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? commentView.Where(c => c.ArticleTitle.Contains(con.Value)) : commentView;
                            commentView = "Username".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? commentView.Where(c => c.Username.Contains(con.Value)) : commentView;
                            commentView = "Status".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? commentView.Where(c => c.Status == Convert.ToInt32(con.Value)) : commentView;
                            //排序
                            if (con.Sort != 0)
                            {
                                if ("CreateTime".Equals(con.Key, StringComparison.OrdinalIgnoreCase))
                                {
                                    if (con.Sort == -1)
                                    {
                                        commentView = commentView.OrderByDescending(a => a.CreateTime);
                                    }
                                    else
                                    {
                                        commentView = commentView.OrderBy(a => a.CreateTime);
                                    }
                                }
                            }
                            else
                            {
                                //默认时间排序
                                commentView = commentView.OrderByDescending(a => a.CreateTime);
                            }
                        }
                    }
                    else
                    {
                        //默认时间排序
                        commentView = commentView.OrderByDescending(a => a.CreateTime);
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
        public async Task<PageList<CommentListView>> GetMyCommentList(int uid, List<SearchCondition>? condidtion, int pageIndex, int pageSize)
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
                    if (condidtion?.Count > 0)
                    {
                        foreach (var con in condidtion)
                        {
                            commentView = "Nick".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? commentView.Where(c => c.Nick.Contains(con.Value)) : commentView;
                            commentView = "Content".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? commentView.Where(c => c.Content.Contains(con.Value)) : commentView;
                            commentView = "ArticleTitle".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? commentView.Where(c => c.ArticleTitle.Contains(con.Value)) : commentView;
                            commentView = "Username".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? commentView.Where(c => c.Username.Contains(con.Value)) : commentView;
                            commentView = "Status".Equals(con.Key, StringComparison.OrdinalIgnoreCase) ? commentView.Where(c => c.Status == Convert.ToInt32(con.Value)) : commentView;
                            //排序
                            if (con.Sort != 0)
                            {
                                if ("CreateTime".Equals(con.Key, StringComparison.OrdinalIgnoreCase))
                                {
                                    if (con.Sort == -1)
                                    {
                                        commentView = commentView.OrderByDescending(a => a.CreateTime);
                                    }
                                    else
                                    {
                                        commentView = commentView.OrderBy(a => a.CreateTime);
                                    }
                                }
                            }
                            else
                            {
                                //默认时间排序
                                commentView = commentView.OrderByDescending(a => a.CreateTime);
                            }
                        }
                    }
                    else
                    {
                        //默认时间排序
                        commentView = commentView.OrderByDescending(a => a.CreateTime);
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
        /// <param name="cid">评论id</param>
        /// <param name="uid">作者id</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<string> AllowComment(int cid, int? uid)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var comment = await dbContext.Comments.FirstOrDefaultAsync(c => c.Id == cid);
                if (uid.HasValue) //是否需要验证文章所属用户
                {
                    //查询评论的这篇文章是否是作者的
                    var article = await articleRpc.GetArticleInfoAsync(new ArticleId { Id = cid });
                    if(article == null || article.Id != comment.ArticleId)
                    {
                        throw new Exception("找不到此条评论");
                    }
                }
                
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

        public void FillUserInfo(List<CommentView> comments,List<UserInfoResponse> users)
        {
            foreach (var comment in comments)
            {
                var u = users.FirstOrDefault(u => u.Id == comment.UserId);
                if(u != null)
                {
                    comment.Username = u.Username;
                    comment.Nick = u.Nick;
                    comment.Photo = u.Photo;
                }
                if(comment.InverseParent.Count > 0)
                {
                    FillUserInfo(comment.InverseParent, users);
                }
            }
        }
    }
}
