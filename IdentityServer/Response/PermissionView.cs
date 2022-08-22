namespace IdentityService.Response
{
    /// <summary>
    /// 权限列表
    /// </summary>
    public class PermissionView
    {
        /// <summary>
        /// 权限ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 服务名
        /// </summary>
        public string Service { get; set; }
        /// <summary>
        /// 控制器
        /// </summary>
        public string Controller { get; set; }
        /// <summary>
        /// 操作
        /// </summary>
        public string Action { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 权限状态
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 是否为公共权限
        /// </summary>
        public int IsPublic { get; set; }
    }
}
