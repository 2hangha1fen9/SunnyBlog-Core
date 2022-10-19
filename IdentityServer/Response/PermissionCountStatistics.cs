namespace IdentityService.Response
{
    public class PermissionCountStatistics
    {
        /// <summary>
        /// 权限数量
        /// </summary>
        public int PermissionCount { get; set; }
        /// <summary>
        /// 角色数量
        /// </summary>
        public int RoleCount { get; set; }
        /// <summary>
        /// 已启用的权限数量
        /// </summary>
        public int EnablePermissionCount { get; set; }
        /// <summary>
        /// 已禁用的权限数量
        /// </summary>
        public int DisablePermissionCount { get; set; }
        /// <summary>
        /// 已启用的角色数量
        /// </summary>
        public int EnableRoleCount { get; set; }
        /// <summary>
        /// 已禁用的角色数量
        /// </summary>
        public int DisableRoleCount { get; set; }
    }
}
