using System.ComponentModel.DataAnnotations;

namespace IdentityService.Request
{
    public class ModifyRoleReq
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        [Required(ErrorMessage = "角色ID不能为空")]
        public int Id { get; set; }
        /// <summary>
        /// 角色名
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int? Status { get; set; }
        /// <summary>
        /// 是否为默认角色
        /// </summary>
        public int? IsDefault { get; set; }
    }
}
