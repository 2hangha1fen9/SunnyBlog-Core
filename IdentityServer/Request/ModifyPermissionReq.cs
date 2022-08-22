using System.ComponentModel.DataAnnotations;

namespace IdentityService.Request
{
    public class ModifyPermissionReq
    {
        /// <summary>
        /// 权限ID
        /// </summary>
        [Required(ErrorMessage = "ID不能为空")]
        public int Id { get; set; }
        /// <summary>
        /// 服务名称
        /// </summary>
        public string? Service { get; set; }
        /// <summary>
        /// 控制器名称
        /// </summary>
        public string? Controller { get; set; }
        /// <summary>
        /// 操作
        /// </summary>
        public string? Action { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// 权限状态
        /// </summary>
        public int? Status { get; set; }
        /// <summary>
        /// 是否为公共权限
        /// </summary>
        public int? IsPublic { get; set; }
    }
}
