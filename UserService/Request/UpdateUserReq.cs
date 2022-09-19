using System.ComponentModel.DataAnnotations;

namespace UserService.Request
{
    /// <summary>
    /// 修改用户信息请求
    /// </summary>
    public class UpdateUserReq
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [Required(ErrorMessage = "ID不能为空")]
        public int Id { get; set; }
        /// <summary>
        /// 登录名
        /// </summary>
        public string? Username { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string? Password { get; set; }   
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
        /// 状态1启用0禁用
        /// </summary>
        public int? Status { get; set; }
    }
}
