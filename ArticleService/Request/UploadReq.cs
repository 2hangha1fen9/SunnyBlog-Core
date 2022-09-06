using System.ComponentModel.DataAnnotations;

namespace ArticleService.Request
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
