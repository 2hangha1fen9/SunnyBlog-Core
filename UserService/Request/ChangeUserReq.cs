using System.ComponentModel.DataAnnotations;

namespace UserService.Request
{
    /// <summary>
    /// 更新用户信息请求
    /// </summary>
    public class ChangeUserReq
    {
        /// <summary>
        /// 昵称
        /// </summary>
        public string? Nick { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public int? Sex { get; set; }
        /// <summary>
        /// 生日
        /// </summary>
        public string? Birthday { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string? Remark { get; set; }
        /// <summary>
        /// 用户留言板
        /// </summary>
        public string? Message { get; set; }
    }
}
