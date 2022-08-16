using Grpc.Core;
using UserService.Rpc.Protos;

namespace UserService.Rpc.Service
{
    /// <summary>
    /// gRPC服务类
    /// </summary>
    public class GUserService:gUser.gUserBase
    {
        /// <summary>
        /// 依赖使用
        /// </summary>
        private readonly UserDBContext dBContext;
        public GUserService(UserDBContext dBContext)
        {
            this.dBContext = dBContext;
        }

        public override Task<UserResponse> GetUserID(UserRequest request, ServerCallContext context)
        {
            //查询用户名密码，将结果存入gRPC载荷对象
            var username = request.Username;
            var password = request.Password;
            var user = dBContext.Users.Where(u => u.Username == username && u.Password == password).FirstOrDefault();
            var userResponse = new UserResponse()
            {
                Id = user.Id,
            };
            return Task.FromResult(userResponse);
        }
    }
}
