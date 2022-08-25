using System.ComponentModel.DataAnnotations;

namespace ArticleService.Request
{
    public class AddTagReq
    {
        /// <summary>
        /// 标签名称
        /// </summary>
        [Required(ErrorMessage = "标签名称不能为空")]
        public string Name { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public int? UserId { get; set; }
        /// <summary>
        /// 标签颜色
        /// </summary>
        public string? Color { get; set; }
        /// <summary>
        /// 是否为私有标签 0公共1私有
        /// </summary>
        public int? IsPrivate { get; set; }
    }
}
