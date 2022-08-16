using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Auth
{
    /// <summary>
    /// 策略要求
    /// </summary>
    public class RBACRequirement: IAuthorizationRequirement
    {
        public string PolicyName { get; }

        public RBACRequirement(string policyName)
        {
            this.PolicyName = policyName;
        }
    }
}
