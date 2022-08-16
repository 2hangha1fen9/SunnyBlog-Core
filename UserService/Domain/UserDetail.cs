using System.ComponentModel.DataAnnotations.Schema;

namespace UserService.Domain
{
    [Table("UserDetail")]
    public class UserDetail
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int userId { get; set; }
        /// <summary>
        /// 用户昵称
        /// </summary>
        public string nick { get; set; }
        /// <summary>
        /// 用户性别
        /// </summary>
        public int sex { get; set; }
        /// <summary>
        /// 用户生日
        /// </summary>
        public DateTime birthday { get; set; }
        /// <summary>
        /// 用户注册时间
        /// </summary>
        public DateTime registerTime { get; set; }
        /// <summary>
        /// 用户备注
        /// </summary>
        public string remark { get; set; }
        /// <summary>
        /// 用户积分
        /// </summary>
        public decimal score { get; set; }
        /// <summary>
        /// 用户头像
        /// </summary>
        public string photo { get; set; }
    }
}
