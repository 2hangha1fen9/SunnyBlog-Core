using System.ComponentModel.DataAnnotations;

namespace UserService.Request
{
    /// <summary>
    /// 忘记密码请求
    /// </summary>
    public class ForgetPasswordReq
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Required(ErrorMessage = "用户名不能为空")]
        public string Username { get; set; }
        /// <summary>
        /// 新密码
        /// </summary>
        [Required(ErrorMessage = "密码不能为空")]
        [StringLength(maximumLength: 20, MinimumLength = 6, ErrorMessage = "密码长度为6~20个字符")]
        public string NewPassword { get; set; }
        /// <summary>
        /// 验证码
        /// </summary>
        [Required(ErrorMessage = "验证码不能为空")]
        [StringLength(maximumLength: 6, MinimumLength = 6, ErrorMessage = "验证码错误")]
        public string VerificationCode { get; set; }
    }
}
