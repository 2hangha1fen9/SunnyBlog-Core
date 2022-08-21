using System.ComponentModel.DataAnnotations;

namespace UserService.Request
{
    public class UbindAccountReq
    {
        /// <summary>
        /// 绑定类型: phone、email
        /// </summary>
        [Required(ErrorMessage = "绑定类型不能为空")]
        public string Type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "密码不能为空")]
        [StringLength(maximumLength: 20, MinimumLength = 6, ErrorMessage = "密码长度为6~20个字符")]
        public string Password { get; set; }

    }
}
