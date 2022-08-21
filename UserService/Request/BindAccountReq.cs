using System.ComponentModel.DataAnnotations;

namespace UserService.Request
{
    /// <summary>
    /// 账户绑定请求
    /// </summary>
    public class BindAccountReq
    {
        /// <summary>
        /// 绑定类型: phone、email
        /// </summary>
        [Required(ErrorMessage = "绑定类型不能为空")]
        public string Type { get; set; }
        /// <summary>
        /// 绑定值
        /// </summary>
        [Required(ErrorMessage = "绑定值不能为空")]
        public string Bind { get; set; }
        /// <summary>
        /// 用户密码
        /// </summary>
        [Required(ErrorMessage = "密码不能为空")]
        [StringLength(maximumLength: 20, MinimumLength = 6, ErrorMessage = "密码长度为6~20个字符")]
        public string Password { get; set; }
        /// <summary>
        /// 验证码
        /// </summary>
        [Required(ErrorMessage = "验证码不能为空")]
        [StringLength(maximumLength: 6, MinimumLength = 6, ErrorMessage = "验证码错误")]
        public string VerificationCode { get; set; }
    }
}
