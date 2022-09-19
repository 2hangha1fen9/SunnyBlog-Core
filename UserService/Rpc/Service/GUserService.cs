using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
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
        private readonly IDbContextFactory<UserDBContext> contextFactory;
        public GUserService(IDbContextFactory<UserDBContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public async override Task<UserInfoResponse> GetUserByPassword(UserRequest request, ServerCallContext context)
        {
            using (var c= contextFactory.CreateDbContext())
            {
                //查询用户名密码，将结果存入gRPC载荷对象
                var username = request.Username;
                var password = request.Password.ShaEncrypt();
                var user = await c.Users.Where(u => ( u.Username == username || u.Email == username || u.Phone ==  username) && u.Password == password).Select(u => new
                {
                    Id = u.Id,
                    Username = u.Username,
                    Nick = u.UserDetail.Nick,
                    Photo = u.Photo
                }).FirstOrDefaultAsync();
                var userResponse = new UserInfoResponse();
                if (user != null)
                {
                    userResponse.Id = user.Id;
                    userResponse.Username = user.Username;
                    userResponse.Nick = user.Nick;
                    userResponse.Photo = user.Photo ?? "";
                }
                return userResponse;
            }
        }

        public async override Task<UserInfoResponse> GetUserByUsername(UserNameRequest request, ServerCallContext context)
        {
            using (var c = contextFactory.CreateDbContext())
            {
                //查询用户名密码，将结果存入gRPC载荷对象
                var username = request.Username;
                var user = await c.Users.Where(u => u.Username == username || u.Email == username || u.Phone == username).Select(u => new
                {
                    Id = u.Id,
                    Username = u.Username,
                    Nick = u.UserDetail.Nick,
                    Photo = u.Photo
                }).FirstOrDefaultAsync();
                var userResponse = new UserInfoResponse();
                if (user != null)
                {
                    userResponse.Id = user.Id;
                    userResponse.Username = user.Username;
                    userResponse.Nick = user.Nick;
                    userResponse.Photo = user.Photo;
                }
                return userResponse;
            }
        }

        public async override Task<UserInfoResponse> GetUserByID(UserInfoRequest request, ServerCallContext context)
        {
            using (var c = contextFactory.CreateDbContext())
            {
                var user = await c.Users.Select(u => new
                {
                    Id = u.Id,
                    Username = u.Username,
                    Nick = u.UserDetail.Nick,
                    Photo = u.Photo
                }).FirstOrDefaultAsync(u => u.Id == request.Id);
                var userInfo = user.MapTo<UserInfoResponse>();
                return userInfo;
            }
        }

        public async override Task<UserInfoListReqponse> GetUserList(Empty request, ServerCallContext context)
        {
            using (var c = contextFactory.CreateDbContext())
            {
                var user = await c.Users.Select(u => new
                {
                    Id = u.Id,
                    Username = u.Username ?? "",
                    Nick = u.UserDetail.Nick ?? "",
                    Photo = u.Photo ?? ""
                }).ToListAsync();
                var userInfoList = new UserInfoListReqponse();
                userInfoList.UserInfo.AddRange(user.MapToList<UserInfoResponse>());
                return userInfoList;
            }
        }
    }
}
