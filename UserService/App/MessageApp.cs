using Microsoft.Extensions.Caching.Distributed;
using System.Text.RegularExpressions;
using TencentCloud.Common;
using TencentCloud.Sms.V20190711;
using TencentCloud.Sms.V20190711.Models;
using UserService.App.Interface;
using UserService.Domain;
using UserService.Domain.config;

namespace Infrastructure
{
    public class MessageApp:IMessageApp
    {

        /// <summary>
        /// Redis缓存客户端
        /// </summary>
        private readonly IDistributedCache cache;
        /// <summary>
        /// 短信配置
        /// </summary>
        private readonly MessageConfig config;

        public MessageApp(IDistributedCache cache,MessageConfig config)
        {
            this.cache = cache;
            this.config = config;
        }

        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<string> SendMessageCode(string phone)
        {
            //生成验证码
            var vcode = EncryptionHelper.GetRandomSequnce(6);
            //配置权限
            Credential cred = new Credential()
            {
                SecretId = config.SecretId,
                SecretKey = config.SecretKey
            };
            //创建api客户端
            SmsClient client = new SmsClient(cred, config.Region);
            //创建api请求
            SendSmsRequest request = new SendSmsRequest();
            request.PhoneNumberSet = new string[] { $"+86{phone}" };
            request.SmsSdkAppid = config.SmsSdkAppId;
            request.TemplateID = config.TemplateId;
            request.TemplateParamSet = new string[] { vcode, "3" };
            request.Sign = config.Sign;
            //发送短信
            SendSmsResponse response = await client.SendSms(request);
            if (response.SendStatusSet[0].Code == "Ok") //发送成功
            {
                //设置验证码有效期
                var option = new DistributedCacheEntryOptions();
                option.SetAbsoluteExpiration(TimeSpan.FromSeconds(120));
                //存入redis中
                await cache.SetStringAsync(phone, EncryptionHelper.GetRandomSequnce(6), option);
                return "发送成功";
            }
            throw new Exception("发送失败");
        }
    }
}
