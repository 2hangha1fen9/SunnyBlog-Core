using System.ComponentModel.DataAnnotations;

namespace UserService.Request
{
    /// <summary>
    /// 积分增加请求
    /// </summary>
    public class IncreaseScoreReq
    {
        /// <summary>
        /// 值
        /// </summary>
        [Required]
        public double Value { get; set; }
        /// <summary>
        /// 原因
        /// </summary>
        [Required]
        public string Reason { get; set; }
    }
}
