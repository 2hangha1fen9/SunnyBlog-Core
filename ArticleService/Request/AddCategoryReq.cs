using System.ComponentModel.DataAnnotations;

namespace ArticleService.Request
{
    public class AddCategoryReq
    {
        /// <summary>
        /// 分类名
        /// </summary>
        [Required(ErrorMessage = "分类名称不能为空")]
        public string Name { get; set; }
        /// <summary>
        /// 父级ID
        /// </summary>
        public int? ParentId { get; set; }
    }
}
