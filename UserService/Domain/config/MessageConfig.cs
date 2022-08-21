namespace UserService.Domain.config
{
    public class MessageConfig
    {
        /// <summary>
        /// 腾讯云账户Id
        /// </summary>
        public string SecretId { get; set; }
        /// <summary>
        /// 腾讯云密钥
        /// </summary>
        public string SecretKey { get; set; }
        /// <summary>
        /// 地域参数
        /// </summary>
        public string Region { get; set; }
        /// <summary>
        /// 短信AppID
        /// </summary>
        public string SmsSdkAppId { get; set; }
        /// <summary>
        /// 短信模板ID
        /// </summary>
        public string TemplateId { get; set; }
        /// <summary>
        /// 签名信息
        /// </summary>
        public string Sign { get; set; }
    }
}
