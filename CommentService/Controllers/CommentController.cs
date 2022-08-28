﻿using CommentService.App.Interface;
using Microsoft.AspNetCore.Mvc;
using Infrastructure;
using CommentService.Request;
using Infrastructure.Auth;
using CommentService.Response;
using System.ComponentModel.DataAnnotations;

namespace CommentService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentApp commentApp;

        public CommentController(ICommentApp commentApp)
        {
            this.commentApp = commentApp;
        }

        /// <summary>
        /// 发表评论
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [RBAC]
        [HttpPost]
        [TypeFilter(typeof(RedisFlush), Arguments = new object[] { new string[] { "*comment*" } })]
        public async Task<Response<string>> Publish(CommentReq request)
        {
            var result = new Response<string>();
            try
            {
                var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "user_id")?.Value;
                result.Result = await commentApp.CommentArticle(request, Convert.ToInt32(userId));
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 删除评论(评论者)
        /// </summary>
        /// <param name="cid"></param>
        /// <returns></returns>
        [RBAC]
        [HttpDelete]
        [TypeFilter(typeof(RedisFlush), Arguments = new object[] { new string[] { "*comment*" } })]
        public async Task<Response<string>> Delete(int cid)
        {
            var result = new Response<string>();
            try
            {
                var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "user_id")?.Value;
                result.Result = await commentApp.DeleteComment(cid, Convert.ToInt32(userId));
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 删除评论(作者)
        /// </summary>
        /// <param name="cid"></param>
        /// <returns></returns>
        [RBAC]
        [HttpDelete]
        [TypeFilter(typeof(RedisFlush), Arguments = new object[] { new string[] { "*comment*" } })]
        public async Task<Response<string>> AuthorDelete(int cid)
        {
            var result = new Response<string>();
            try
            {
                var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "user_id")?.Value;
                result.Result = await commentApp.DeleteArticleComment(cid, Convert.ToInt32(userId));
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 删除评论(管理员)
        /// </summary>
        /// <param name="cid"></param>
        /// <returns></returns>
        [RBAC]
        [HttpDelete]
        [TypeFilter(typeof(RedisFlush), Arguments = new object[] { new string[] { "*comment*" } })]
        public async Task<Response<string>> Remove(int cid)
        {
            var result = new Response<string>();
            try
            {
                result.Result = await commentApp.DeleteComment(cid);
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 获取文章评论
        /// </summary>
        /// <param name="aid">文章ID</param>
        /// <param name="condidtion">过滤条件</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [RBAC(IsPublic = 1)]
        [HttpGet]
        [TypeFilter(typeof(RedisCache))]
        public async Task<Response<PageList<CommentView>>> List([FromQuery]int aid, [FromBody]List<SearchCondition>? condidtion = null, [FromQuery] int? pageIndex = 1, [FromQuery] int? pageSize = 10)
        {
            var result = new Response<PageList<CommentView>>();
            try
            {
                result.Result = await commentApp.GetArticleComment(aid,condidtion,pageIndex.Value,pageSize.Value);
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 获取用户发表的评论
        /// </summary>
        /// <param name="aid">文章ID</param>
        /// <param name="condidtion">过滤条件</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [RBAC(IsPublic = 1)]
        [HttpGet]
        public async Task<Response<PageList<CommentListView>>> User([FromQuery][Required]int uid, [FromBody]List<SearchCondition>? condidtion = null, [FromQuery]int? pageIndex = 1, [FromQuery]int? pageSize = 10)
        {
            var result = new Response<PageList<CommentListView>>();
            try
            {
                result.Result = await commentApp.GetUserCommentList(uid, condidtion, pageIndex.Value, pageSize.Value);
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 获取用户给我的的评论
        /// </summary>
        /// <param name="aid">文章ID</param>
        /// <param name="condidtion">过滤条件</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [RBAC]
        [HttpGet]
        public async Task<Response<PageList<CommentListView>>> My([FromBody] List<SearchCondition>? condidtion = null, [FromQuery] int? pageIndex = 1, [FromQuery] int? pageSize = 10)
        {
            var result = new Response<PageList<CommentListView>>();
            try
            {
                var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "user_id")?.Value;
                result.Result = await commentApp.GetMyCommentList(Convert.ToInt32(userId), condidtion, pageIndex.Value, pageSize.Value);
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 获取所有评论
        /// </summary>
        /// <returns></returns>
        [RBAC]
        [HttpGet]
        public async Task<Response<PageList<CommentListView>>> All([FromBody] List<SearchCondition> condidtion = null, [FromQuery] int? pageIndex = 1, [FromQuery] int? pageSize = 10)
        {
            var result = new Response<PageList<CommentListView>>();
            try
            {
                result.Result = await commentApp.GetCommentList(condidtion, pageIndex.Value, pageSize.Value);
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }
    
        /// <summary>
        /// 读评论
        /// </summary>
        /// <param name="cid"></param>
        /// <returns></returns>
        [RBAC]
        [HttpPut]
        public async Task<Response<string>> Read(int cid)
        {
            var result = new Response<string>();
            try
            {
                var userId = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == "user_id")?.Value;
                result.Result = await commentApp.ReadArticle(cid, Convert.ToInt32(userId));
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }
    }
}