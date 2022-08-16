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
                //微服务客户端
                new Client
                {
                    ClientId = "client",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                },
                //Vue客户端
                new Client
                {
                    ClientId = "vue",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    AccessTokenType = AccessTokenType.Jwt,
                    AllowOfflineAccess = true,
                    RedirectUris = { "http://localhost:8080/callback" },
                    PostLogoutRedirectUris = { "http://localhost:8080/home/index" },
                    AllowedCorsOrigins = { "http://localhost:8080" },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                    }
                },
                //开发客户端
                new Client
                {
                    ClientId = "dev",
                    RequireClientSecret=false,
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    }
                }
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
