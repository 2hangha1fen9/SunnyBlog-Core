using System.ComponentModel.DataAnnotations;

namespace IdentityService.Request
{
    public class AddRoleReq
    {
        /// <summary>
        /// 角色名
        /// </summary>
        [Required(ErrorMessage = "角色不能为空")] 
        public string Name { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int? Status { get; set; } = 1;
    }
}
