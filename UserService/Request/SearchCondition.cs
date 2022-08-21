using System.ComponentModel.DataAnnotations;

namespace UserService.Request
{
    public class SearchCondition
    {
        [Required(ErrorMessage = "搜索条件不能为空")]
        public string Key { get; set; }
        [Required(ErrorMessage = "搜索值不能为空")]
        public string Value { get; set; }
    }
}
