﻿namespace UserService.Response
{
    /// <summary>
    /// Api视图
    /// </summary>
    public class UserView
    {
        /// <summary>
        /// 昵称
        /// </summary>
        public string Nick { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        public string Photo { get; set; }
    }
}
