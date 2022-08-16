using System.ComponentModel.DataAnnotations.Schema;

namespace UserService.Domain
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
        /// 用户手机
        /// </summary>
        [Column("phone")]
        public string Phone { get; set; }
        /// <summary>
        /// 用户邮箱
        /// </summary>
        [Column("email")]
        public string? Email { get; set; }

        /// <summary>
        /// 用户密码
        /// </summary>
        [Column("password")]
        public string Password { get; set; }

        /// <summary>
        /// 用户状态
        /// </summary>
        [Column("status")]
        public int Status { get; set; }
    }
}
