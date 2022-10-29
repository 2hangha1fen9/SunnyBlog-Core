using System.ComponentModel.DataAnnotations;

namespace UserService.Request
{
    public class UserRegisterReq
    {
        /// <summary>
        /// 登录名
        /// </summary>
        [Required(ErrorMessage = "用户不能为空")]
        [StringLength(maximumLength:20,MinimumLength =6,ErrorMessage = "用户名长度为6~20个字符")]
        public string Username { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [Required(ErrorMessage = "密码不能为空")]
        [StringLength(maximumLength: 20, MinimumLength = 6, ErrorMessage = "密码长度为6~20个字符")]
        public string Password { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string? Email { get; set; }
        /// <summary>
        /// 手机
        /// </summary>
        public string? Phone { get; set; }
        /// <summary>
        /// 验证码
        /// </summary>
        [Required(ErrorMessage = "验证码不能为空")]
        [StringLength(maximumLength: 6, MinimumLength = 6,ErrorMessage = "验证码错误")]
        public string VerificationCode { get; set; }
    }
}
