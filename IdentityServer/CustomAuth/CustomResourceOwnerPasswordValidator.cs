using IdentityModel;
using Service.IdentityService.App.Interface;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using System.Security.Claims;
using Newtonsoft.Json;
using Service.IdentityService.Domain;
using System.Collections;

namespace Service.IdentityService
{
    /// <summary>
    /// 自定义令牌信息
    /// </summary>
    public class CustomResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        //依赖使用
        private readonly IPermissionApp permissionApp;
        public CustomResourceOwnerPasswordValidator(IPermissionApp permissionApp)
        {
            this.permissionApp = permissionApp;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            if (!string.IsNullOrEmpty(context.UserName) && !string.IsNullOrEmpty(context.Password))
            {
                //查询用户权限
                var data = await permissionApp.GetPermission(context.UserName, context.Password);
                var id = data[0].ToString();
                var permission = data[1] as Array;
                if (!string.IsNullOrEmpty(id) && permission.Length > 0)
                {
                    //自定义令牌信息，将用户id和权限表存入token
                    context.Result = new GrantValidationResult(context.UserName,
                        OidcConstants.AuthenticationMethods.Password,
                        new Claim[] {
                            new Claim("user_id",id),
                            new Claim("permission",JsonConvert.SerializeObject(permission))
                        });
                }
                else
                {
                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "认证失败");
                }
            }
        }
    }
}
