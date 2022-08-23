using System.ComponentModel.DataAnnotations;

namespace IdentityService.Request
{
    public class UserRoleBindReq
    {
        [Required(ErrorMessage = "用户ID不能为空")]
        public int UserId { get; set; }
        [Required(ErrorMessage = "角色ID不能为空")]
        public int[] RoleIds { get; set; }
    }
}
