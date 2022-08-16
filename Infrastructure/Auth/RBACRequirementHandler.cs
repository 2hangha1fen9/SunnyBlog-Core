using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Auth
{
    /// <summary>
    /// RBAC授权控制处理
    /// </summary>
    public class RBACRequirementHandler : AuthorizationHandler<RBACRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public RBACRequirementHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// 授权处理逻辑
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requirement"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RBACRequirement requirement)
        {
            //获取token中的用户信息
            var subid = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //获取router对象
            var router = _httpContextAccessor.HttpContext?.GetRouteData();
            //获取Controller、Action
            var currentController = router?.Values["controller"]?.ToString()?.ToLower();
            var currentAction = router?.Values["action"]?.ToString()?.ToLower();
            //获取入口程序集，标识api
            if (string.IsNullOrWhiteSpace(subid) == false && string.IsNullOrWhiteSpace(currentController) == false && string.IsNullOrWhiteSpace(currentAction) == false)
            {
                //获取token权限信息
                string permissions = context.User.Claims.FirstOrDefault(c => c.Type == "permission").Value;
                List<Permission> permissionsList = JsonConvert.DeserializeObject<List<Permission>>(permissions);
                if (permissionsList.Find(p => ((p.Controller == "*" || p.Controller == currentController) && (p.Action == "*" || p.Action == currentAction))) != null)
                {
                    context.Succeed(requirement); //放行请求
                }
                else
                {
                    context.Fail(); //拒绝请求
                }
            }
            return Task.CompletedTask;
        }
    }
}
