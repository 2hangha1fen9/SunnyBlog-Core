using System.ComponentModel.DataAnnotations.Schema;

namespace Service.IdentityService.Domain
{
    [Table("RolePermissionRelation")]
    public class RolePermissionRelation
    {
        /// <summary>
        /// 角色权限ID
        /// </summary>
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        [Column("roleId")]
        public int RoleId { get; set; }

        /// <summary>
        /// 权限ID
        /// </summary>
        [Column("permissionId")]
        public int PermissionId { get; set; }

    }
}
