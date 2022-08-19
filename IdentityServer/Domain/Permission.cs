using System.ComponentModel.DataAnnotations.Schema;

namespace Service.IdentityService.Domain
{
    [Table("Permission")]
    public class Permission
    {
        /// <summary>
        /// 权限ID
        /// </summary>
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        /// 服务名称
        /// </summary>
        [Column("service")]
        public string Service { get; set; }

        /// <summary>
        /// 控制器名称
        /// </summary>
        [Column("controller")]
        public string Controller { get; set; }

        /// <summary>
        /// 控制器名称
        /// </summary>
        [Column("action")]
        public string Action { get; set; }

        /// <summary>
        /// 权限描述
        /// </summary>
        [Column("description")]
        public string Description { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Column("createTime")]
        public DateTime CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 更新时间
        /// </summary>
        [Column("updateTime")]
        public DateTime? UpdateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 资源对象
        /// </summary>
        [Column("status")]
        public int Status { get; set; } = 1;
    }
}
