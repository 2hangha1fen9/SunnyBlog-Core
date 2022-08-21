using System.ComponentModel.DataAnnotations;

namespace UserService.Request
{
    /// <summary>
    /// 用户头像上传请求
    /// </summary>
    public class UploadPhotoReq
    {
        /// <summary>
        /// 图片数据
        /// </summary>
        [Required]
        public string Data { get; set; }
    }
}
