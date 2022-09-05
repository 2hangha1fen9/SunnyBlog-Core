namespace IdentityService.Response
{
    public class RoleView
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 角色名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 角色状态
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 是否为默认角色
        /// </summary>
        public int IsDefault { get; set; } 
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
    }
}
