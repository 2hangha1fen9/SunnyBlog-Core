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
        /// 用户昵称
        /// </summary>
        [Column("nick")]
        public string? Nick { get; set; }
        /// <summary>
        /// 用户密码
        /// </summary>
        [Column("password")]
        public string Password { get; set; }
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
        /// 用户性别
        /// </summary>
        [Column("sex")]
        public int? Sex { get; set; }
        /// <summary>
        /// 用户生日
        /// </summary>
        [Column("birthday")]
        public DateTime? Birthday { get; set; }
        /// <summary>
        /// 用户注册时间
        /// </summary>
        [Column("registerTime")]
        public DateTime RegisterTime { get; set; }
        /// <summary>
        /// 用户备注
        /// </summary>
        [Column("remark")]
        public string? Remark { get; set; }
        /// <summary>
        /// 用户积分
        /// </summary>
        [Column("score")]
        public decimal Score { get; set; }
    }
}
