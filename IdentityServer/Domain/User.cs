using System.ComponentModel.DataAnnotations.Schema;

namespace Service.IdentityService.Domain
{
    [Table("User")]
    public class User
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [Column("id")]
        public int Id { get; set; }
        /// <summary>
        /// 用户账号
        /// </summary>
        [Column("username")]
        public string Username { get; set; }
        /// <summary>
        /// 用户密码
        /// </summary>
        [Column("password")]
        public string Password { get; set; }
    }
}
