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
        /// 代码块主题
        /// </summary>
        public string? CodeStyle { get; set; }
        /// <summary>
        /// 内容主题
        /// </summary>
        public string? ContentStyle { get; set; } 
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
        public int? RegionId { get; set; }
        /// <summary>
        /// 文章分类
        /// </summary>
        public int? CategoryId { get; set; }
        /// <summary>
        /// 文章状态-1待审核1已发布2私有3回收站
        /// </summary>
        public int? Status { get; set; }
        /// <summary>
        /// 评论状态-1不可以评论1可评论2需要审核评论
        /// </summary>
        public int? CommentStatus { get; set; }
    }
}
