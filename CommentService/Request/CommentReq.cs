using System.ComponentModel.DataAnnotations;

namespace CommentService.Request
{
    /// <summary>
    /// 评论请求
    /// </summary>
    public class CommentReq
    {
        /// <summary>
        /// 文章ID
        /// </summary>
        [Required(ErrorMessage = "文章Id不能为空")]
        public int ArticleId { get; set; }
        /// <summary>
        /// 父级评论ID
        /// </summary>
        public int? ParentId { get; set; }
        /// <summary>
        /// 评论内容
        /// </summary>
        [Required(ErrorMessage = "评论内容不能为空")]
        public string Content { get; set; }
    }
}
