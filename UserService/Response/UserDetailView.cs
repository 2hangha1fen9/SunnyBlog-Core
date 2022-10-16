namespace UserService.Response
{
    /// <summary>
    /// 用户详情响应视图
    /// </summary>
    public class UserDetailView
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 登录名
        /// </summary>
        public string Username { get; set; }
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
        public DateTime? Birthday { get; set; }
        /// <summary>
        /// 注册时间
        /// </summary>
        public DateTime? RegisterTime { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string? Remark { get; set; }
        /// <summary>
        /// 用户留言版
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 积分
        /// </summary>
        public decimal Score { get; set; }
        /// <summary>
        /// 粉丝
        /// </summary>
        public int Fans { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        public string? Photo { get; set; }
        /// <summary>
        /// 封面
        /// </summary>
        public string? Cover { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }

    }
}
