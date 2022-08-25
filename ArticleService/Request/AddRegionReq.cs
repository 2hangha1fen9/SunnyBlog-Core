using System.ComponentModel.DataAnnotations;

namespace ArticleService.Request
{
    public class AddRegionReq
    {
        /// <summary>
        /// 分区名
        /// </summary>
        [Required(ErrorMessage = "分区名称不能为空")]
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// 父级ID
        /// </summary>
        public int? ParentId { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int? Status { get; set; }

    }
}
