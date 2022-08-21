using Microsoft.Extensions.Caching.Distributed;
using MimeKit;
using MailKit.Net.Smtp;
using System.Text.RegularExpressions;
using UserService.Domain;
using MailKit.Security;
using UserService.App.Interface;
using UserService.Domain.config;

namespace Infrastructure
{
    public class MailApp:IMailApp
    {
        /// <summary>
        /// 邮件配置
        /// </summary>
        private readonly  MailConfig config;
        /// <summary>
        /// 邮件模板
        /// </summary>
        private readonly MailTemplate template;
        /// <summary>
        /// Redis缓存客户端
        /// </summary>
        private readonly IDistributedCache cache;

        public MailApp(MailConfig config,MailTemplate template,IDistributedCache cache)
        {
            this.config = config;
            this.template = template;
            this.cache = cache;
        }

        public async Task<string> SendEmailCode(string email)
        {
            //生成验证码
            var vcode = EncryptionHelper.GetRandomSequnce(6);
            //设置邮件
            var mime = new MimeMessage();
            //设置发件人
            mime.From.Add(MailboxAddress.Parse(config.Mail));
            mime.Sender = MailboxAddress.Parse(config.Mail);
            //设置收件人
            mime.To.Add(MailboxAddress.Parse(email));
            //设置标题
            mime.Subject = template.Subject;
            //设置正文
            var body = new BodyBuilder();
            body.HtmlBody = string.Format(template.Body, vcode);
            mime.Body = body.ToMessageBody();
            //发送邮箱
            using (var smtp = new SmtpClient())
            {
                //设置邮件服务器地址
                smtp.Connect(config.Host, config.Port, SecureSocketOptions.StartTls);
                //设置权限
                await smtp.AuthenticateAsync(config.Mail, config.Password);
                //发送邮件
                await smtp.SendAsync(mime);
            }
            //设置验证码有效期
            var option = new DistributedCacheEntryOptions();
            option.SetAbsoluteExpiration(TimeSpan.FromSeconds(180));
            //存入redis中
            await cache.SetStringAsync(email, vcode, option);
            return "发送成功";
        }
    }
}
