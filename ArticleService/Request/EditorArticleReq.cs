using System.ComponentModel.DataAnnotations;

namespace ArticleService.Request
{
    /// <summary>
    /// 编辑文章请求
    /// </summary>
    public class EditorArticleReq
    {
        /// <summary>
        /// 文章ID
        /// </summary>
        [Required(ErrorMessage = "文章ID不能为空")]
        public int Id { get; set; }
        /// <summary>
        /// 文章标题
        /// </summary>
        public string? Title { get; set; }
        /// <summary>
        /// 文章内容
        /// </summary>
        public string? Content { get; set; }
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
        public List<int>? Categorys { get; set; }
        /// <summary>
        /// 文章状态-1待审核1已发布2私有3回收站
        /// </summary>
        public int? Status { get; set; }
        /// <summary>
        /// 评论状态-1不可以评论1可评论2需要审核评论
        /// </summary>
        public int? CommentStatus { get; set; }
        /// <summary>
        /// 是否被锁定1未锁定-1锁定
        /// </summary>
        public int? isLock { get; set; }
        
    }
}
