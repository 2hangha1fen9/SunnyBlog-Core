using System.ComponentModel.DataAnnotations;

namespace IdentityService.Request
{
    public class RolePermissionBindReq
    {
        [Required(ErrorMessage = "角色Id不能为空")]
        public int RoleId { get; set; }
        [Required(ErrorMessage = "权限Id不能为空")]
        public int[] PermissionIds { get; set; }
    }
}
