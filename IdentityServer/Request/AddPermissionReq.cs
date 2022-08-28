using System.ComponentModel.DataAnnotations;

namespace IdentityService.Request
{
    public class AddPermissionReq
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        [Required(ErrorMessage = "服务名不能为空")]
        public string Service { get; set; }
        /// <summary>
        /// 控制器名称
        /// </summary>
        [Required(ErrorMessage = "控制器不能为空")]
        public string Controller { get; set; }
        /// <summary>
        /// 操作
        /// </summary>
        [Required(ErrorMessage = "操作不能为空")]
        public string Action { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// 权限状态
        /// </summary>
        public int? Status { get; set; } = 1;
        /// <summary>
        /// 是否为公共权限
        /// </summary>
        public int IsPublic { get; set; } = -1;
    }
}
