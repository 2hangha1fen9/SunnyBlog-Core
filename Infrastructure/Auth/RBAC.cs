using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Auth
{
    /// <summary>
    /// 权限标识
    /// </summary>
    public class RBAC: AuthorizeAttribute
    {
        public RBAC(string policyName = "")
        {
            this.PolicyName = policyName;
        }

        public string PolicyName
        {
            get
            {
                return PolicyName;
            }
            set
            {
                Policy = $"RBAC{value.ToString()}";
            }
        }
    }
}
