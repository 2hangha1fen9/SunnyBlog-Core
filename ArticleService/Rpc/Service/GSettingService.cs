using ArticleService.Rpc.Protos;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace ArticleService.Rpc.Service
{
    public class GSettingService : gSetting.gSettingBase
    {
        private readonly IDbContextFactory<ArticleDBContext> contextFactory;

        public GSettingService(IDbContextFactory<ArticleDBContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        //获取所有设置项
        public async override Task<SettingList> GetGlobalSetting(Empty request, ServerCallContext context)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var settings = await dbContext.GlobalSettings.ToListAsync();
                if (settings != null)
                {
                    var settingList = new SettingList();
                    settingList.Settings.AddRange(settings.MapToList<Setting>());
                    return settingList;
                }
                return new SettingList();
            }
        }
    }
}
