using System.ComponentModel.DataAnnotations;

namespace CommentService.Request
{
    //图片上传
    public class UploadReq
    {
        /// <summary>
        /// 图片数据
        /// </summary>
        [Required]
        public IFormFile Data { get; set; }
    }
}
