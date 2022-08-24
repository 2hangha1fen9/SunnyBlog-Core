using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class SearchCondition
    {
        [Required(ErrorMessage = "搜索条件不能为空")]
        public string Key { get; set; }
        [Required(ErrorMessage = "搜索值不能为空")]
        public string Value { get; set; }
    }
}
