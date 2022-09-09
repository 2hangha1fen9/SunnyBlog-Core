using System.ComponentModel.DataAnnotations;

namespace ArticleService.Request
{
    public class UpdateTagReq
    {
        /// <summary>
        /// 标签ID
        /// </summary>
        [Required(ErrorMessage = "标签ID不能为空")]
        public int Id { get; set; }
        /// <summary>
        /// 标签名
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// 颜色
        /// </summary>
        public string? Color { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public int? UserId { get; set; }
        /// <summary>
        /// 是否为私有标签
        /// </summary>
        public int? IsPrivate { get; set; }
    }
}
