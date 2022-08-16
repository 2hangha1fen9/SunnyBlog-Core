using System.ComponentModel.DataAnnotations.Schema;

namespace Service.IdentityService.Domain
{
    [Table("Role")]
    public class Role
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        /// 角色名
        /// </summary>
        [Column("name")]
        public string Name { get; set; }

        /// <summary>
        /// 角色状态
        /// </summary>
        [Column("status")]
        public int Status { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Column("createTime")]
        public DateTime? CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
    }
}
