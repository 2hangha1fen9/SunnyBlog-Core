using System.ComponentModel.DataAnnotations;

namespace UserService.Request.Admin
{
    public class AddUserReq
    {
        /// <summary>
        /// 登录名
        /// </summary>
        [Required(ErrorMessage = "用户不能为空")]
        public string Username { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [Required(ErrorMessage = "密码不能为空")]
        public string Password { get; set; }
        /// <summary>
        /// 用户昵称
        /// </summary>
        public string? Nick { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string? Phone { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string? Email { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public int? Sex { get; set; }
        /// <summary>
        /// 生日
        /// </summary>
        public string? Birthday { get; set; }
        /// <summary>
        /// 注册时间
        /// </summary>
        public string? RegisterTime { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string? Remark { get; set; }
        /// <summary>
        /// 积分
        /// </summary>
        public decimal? Score { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        public string? Photo { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int? Status { get; set; }
    }
}
