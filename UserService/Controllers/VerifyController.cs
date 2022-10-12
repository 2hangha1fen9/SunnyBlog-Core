using Infrastructure;
using Infrastructure.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserService.App.Interface;
using UserService.Request;

namespace UserService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class VerifyController : ControllerBase
    {
        private readonly IMailApp mailApp;
        private readonly IMessageApp messageApp;

        public VerifyController(IMailApp mailApp, IMessageApp messageApp)
        {
            this.mailApp = mailApp;
            this.messageApp = messageApp;
        }

        /// <summary>
        /// 发送验证码
        /// </summary>
        /// <param name="type"></param>
        /// <param name="receiver"></param>
        /// <returns></returns>
        [HttpPost]
        [RBAC(IsPublic = 1)]
        public async Task<Response<string>> SendVCode(SendVCodeReq request)
        {
            var result = new Response<string>();
            try
            {
                if (!string.IsNullOrWhiteSpace(request.Type) && request.Type == "phone")
                {
                    result.Result = await messageApp.SendMessageCode(request.Receiver);
                }
                else if (!string.IsNullOrWhiteSpace(request.Type) && request.Type == "email")
                {
                    result.Result = await mailApp.SendEmailCode(request.Receiver);
                }
                else
                {
                    result.Result = "发送渠道错误:必须为phone、email";
                }
            }
            catch (Exception ex)
            {
                result.Status = 500;
                result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return result;
        }
    }
}
