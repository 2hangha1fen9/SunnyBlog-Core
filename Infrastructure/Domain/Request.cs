using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    /// <summary>
    /// 通用搜索条件
    /// </summary>
    public class SearchCondition
    {
        [Required(ErrorMessage = "搜索条件不能为空")]
        public string Key { get; set; }
        [Required(ErrorMessage = "搜索值不能为空")]
        public string Value { get; set; }
        /// <summary>
        /// 是否对条件进行排序-1降序 0不排序 1升序
        /// </summary>
        public int? Sort { get; set; } = 0;
    }
}
