using IdentityModel;
using Service.IdentityService.App.Interface;
using IdentityServer4.Validation;
using System.Security.Claims;
using Newtonsoft.Json;
using System.Collections;
using Infrastructure;
using IdentityModel.Client;
using IdentityService.Rpc.Protos;
using UserInfoResponse = IdentityService.Rpc.Protos.UserInfoResponse;
using IdentityServer4.Models;
using StackExchange.Redis;

namespace Service.IdentityService
{
    /// <summary>
    /// 自定义令牌信息
    /// </summary>
    public class CustomResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        //依赖使用
        private readonly IPermissionApp permissionApp;
        private readonly IDatabase database;

        public CustomResourceOwnerPasswordValidator(IPermissionApp permissionApp, IConnectionMultiplexer connection)
        {
            this.permissionApp = permissionApp;
            this.database= connection.GetDatabase();
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            if (!string.IsNullOrEmpty(context.UserName) && !string.IsNullOrEmpty(context.Password))
            {
                UserInfoResponse? user = null;
                Array? permission = null;
                //账号密码模式查询用户权限
                if (context.Request.ClientId == "password")
                {
                    var data = await permissionApp.GetPermission(context.UserName, context.Password);
                    user = data[0] as UserInfoResponse;
                    permission = data[1] as Array;
                }
                //验证码查询用户权限
                else if (context.Request.ClientId == "vcode")
                {
                    //验证邮箱/手机号验证码
                    var code = await database.StringGetDeleteAsync($"VCode:{context.UserName}");
                    if (code == context.Password)
                    {
                        var data = await permissionApp.GetPermission(context.UserName);
                        user = data[0] as UserInfoResponse;
                        permission = data[1] as Array;
                    }
                }
                if (user != null && permission != null && permission?.Length > 0)
                {
                    //自定义令牌信息，将用户id和权限表存入token
                    context.Result = new GrantValidationResult(context.UserName,
                        OidcConstants.AuthenticationMethods.Password,
                        new Claim[] {
                            new Claim("user_id",user.Id.ToString()),
                            new Claim("user_name",user.Username),
                            new Claim("user_nick",user.Nick),
                            new Claim("user_photo",user.Photo),
                            new Claim("permission",JsonConvert.SerializeObject(permission))
                        });
                }
                else
                {
                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "鉴权失败");
                }
            }
        }
    }
}
