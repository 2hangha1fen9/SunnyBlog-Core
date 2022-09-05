using IdentityServer4;
using IdentityServer4.Models;

namespace IdentityServer
{
    //IDS4服务配置
    public class Config
    {
        /// <summary>
        /// 客户端资源
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Client> Clients()
        {
            return new[]
            {
                //账号密码登录
                new Client
                {
                    ClientId = "password",
                    RequireClientSecret=false,
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    }
                },
                //邮箱/手机号登录
                new Client
                {
                    ClientId = "vcode",
                    RequireClientSecret=false,
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    }
                },
            };
        }

        /// <summary>
        /// 认证资源
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<IdentityResource> IdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
        }
    }
}
