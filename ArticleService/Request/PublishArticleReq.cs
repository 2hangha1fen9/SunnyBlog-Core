using System.ComponentModel.DataAnnotations;

namespace ArticleService.Request
{
    /// <summary>
    /// 发布文章请求
    /// </summary>
    public class PublishArticleReq
    {
        /// <summary>
        /// 文章标题
        /// </summary>
        [Required(ErrorMessage = "文章标题不能为空")]
        public string Title { get; set; }
        /// <summary>
        /// 文章内容摘要
        /// </summary>
        [Required(ErrorMessage = "文章内容不能为空")]
        public string Content { get; set; }
        /// <summary>
        /// 文章封面
        /// </summary>
        public string? Photo { get; set; }
        /// <summary>
        /// 文章标签
        /// </summary>
        public List<int>? Tags { get; set; }
        /// <summary>
        /// 文章分区
        /// </summary>
        public int? Region { get; set; }
        /// <summary>
        /// 文章分类
        /// </summary>
        public List<int>? Categorys { get; set; }
        /// <summary>
        /// 文章状态
        /// </summary>
        public int? Status { get; set; }
        /// <summary>
        /// 评论状态
        /// </summary>
        public int? CommentStatus { get; set; }
    }
}
