using IdentityModel;
using Service.IdentityService.App.Interface;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using System.Security.Claims;

namespace Service.IdentityService
{
    /// <summary>
    /// 自定义令牌信息
    /// </summary>
    public class CustomResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        //依赖使用
        private readonly IUserApp userApp;
        private readonly IPermissionApp permissionApp;
        public CustomResourceOwnerPasswordValidator(IUserApp userApp, IPermissionApp permissionApp)
        {
            this.userApp = userApp;
            this.permissionApp = permissionApp;
        }

        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            if (!string.IsNullOrEmpty(context.UserName) && !string.IsNullOrEmpty(context.Password))
            {
                //获取请求的账号密码信息
                var loginUser = userApp.GetUser(context.UserName, context.Password);
                if (loginUser != null)
                {
                    //自定义令牌信息，将用户id和权限表存入token
                    context.Result = new GrantValidationResult(loginUser.Username,
                        OidcConstants.AuthenticationMethods.Password,
                        new Claim[] {new Claim("permission",permissionApp.GetPermission(loginUser))
                        }); ;
                        return Task.CompletedTask; 
                }
                else
                {
                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant,"认证失败");
                }
            }
            return Task.CompletedTask;
        }
    }
}
