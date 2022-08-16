using System.ComponentModel.DataAnnotations.Schema;

namespace Service.IdentityService.Domain
{
    [Table("UserRoleRelation")]
    public class UserRoleRelation
    {
        /// <summary>
        /// 用户角色关系ID
        /// </summary>
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [Column("userId")]
        public int UserId { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        [Column("roleId")]
        public int RoleId { get; set; }
    }
}
