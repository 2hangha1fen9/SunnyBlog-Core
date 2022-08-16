using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Auth
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
        /// 资源对象
        /// </summary>
        [Column("status")]
        public int Status { get; set; }
    }
}
