using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Auth
{
    /// <summary>
    /// 策略提供
    /// </summary>
    public class RBACPolicyProvider : IAuthorizationPolicyProvider
    {
        private readonly IConfiguration _configuration;
        public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }

        public RBACPolicyProvider(IConfiguration configuration, IOptions<AuthorizationOptions> options)
        {
            _configuration = configuration;
            FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }


        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            return FallbackPolicyProvider.GetDefaultPolicyAsync();
        }

        public Task<AuthorizationPolicy> GetFallbackPolicyAsync()
        {
            return Task.FromResult<AuthorizationPolicy>(null);
        }

        /// <summary>
        /// 自定义Requirement
        /// </summary>
        /// <param name="policyName"></param>
        /// <returns></returns>
        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith("RBAC", StringComparison.OrdinalIgnoreCase))
            {
                var policys = new AuthorizationPolicyBuilder();
                //这里使用自定义Requirement
                policys.AddRequirements(new RBACRequirement(policyName.Replace("RABC", "")));
                return Task.FromResult(policys.Build());
            }

            return Task.FromResult<AuthorizationPolicy>(null);

        }
    }
}
