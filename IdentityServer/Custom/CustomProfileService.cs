using IdentityServer4.Models;
using IdentityServer4.Services;

namespace Service.IdentityService.Custom
{
    public class CustomProfileService : IProfileService
    {
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            //将自定义令牌替换
            var claims = context.Subject.Claims.ToList();
            context.IssuedClaims = claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;
        }
    }
}
