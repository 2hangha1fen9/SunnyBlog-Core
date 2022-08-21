using System.ComponentModel.DataAnnotations;

namespace UserService.Request
{
    /// <summary>
    /// 发送验证码请求
    /// </summary>
    public class SendVCodeReq
    {
        /// <summary>
        /// 发送类型，手机phone，邮箱mail
        /// </summary>
        [Required(ErrorMessage = "类型必须是phone、email")]
        public string Type { get; set; }
        /// <summary>
        /// 接受人
        /// </summary>
        [Required(ErrorMessage = "接收人不能为空")]
        public string Receiver { get; set; }
    }
}
