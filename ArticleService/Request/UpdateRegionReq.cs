using System.ComponentModel.DataAnnotations;

namespace ArticleService.Request
{
    public class UpdateRegionReq
    {
        /// <summary>
        /// 分区ID
        /// </summary>
        [Required(ErrorMessage = "分区ID不能为空")]
        public int Id { get; set; }
        /// <summary>
        /// 分区名称
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// 分区描述
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
