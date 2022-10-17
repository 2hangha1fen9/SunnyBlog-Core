namespace UserService.Response
{
    /// <summary>
    /// Api视图
    /// </summary>
    public class UserView
    {
        public int Id { get; set; }
        /// <summary>
        /// 用户名称
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string Nick { get; set; }
        /// <summary>
        /// 简介
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        public string Photo { get; set; }
        /// <summary>
        /// 封面
        /// </summary>
        public string Cover { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public int Sex { get; set; }
        /// <summary>
        /// 粉丝
        /// </summary>
        public int Fans { get; set; }
        /// <summary>
        /// 关注数
        /// </summary>
        public int Follows { get; set; }
        /// <summary>
        /// 用户留言版
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 注册时间
        /// </summary>
        public DateTime RegisterTime { get; set; }
    }
}
