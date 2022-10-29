using Infrastructure.Consul;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using UserService;
using UserService.App;
using UserService.App.Interface;
using Infrastructure.Auth;
using Microsoft.IdentityModel.Tokens;
using UserService.Rpc.Service;
using Com.Ctrip.Framework.Apollo;
using Infrastructure;
using Com.Ctrip.Framework.Apollo.Enums;
using Microsoft.Extensions.FileProviders;
using UserService.Domain.config;
using StackExchange.Redis;
using IdentityService.Rpc.Protos;
using System.Net;

var builder = WebApplication.CreateBuilder(args);
var useSkywalking = builder.Configuration.GetValue<bool>("useSkywalking");
var useApollo = builder.Configuration.GetValue<bool>("useApollo");
var port = builder.Configuration.GetValue<int>("port");

if (useSkywalking)
{
    builder.Services.AddSkyAPM();
}
if (useApollo)
{
    //Apollo配置中心
    builder.Host.ConfigureAppConfiguration((context, builder) =>
    {
        builder.AddApollo(builder.Build()
                .GetSection("Apollo"))
                .AddDefault()
                .AddNamespace("UserService", ConfigFileFormat.Json);
    });
}

//配置端口
builder.WebHost.UseUrls($"https://*:{port}");


// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    //配置接口注释
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    
});

//邮件配置注册
builder.Services.AddSingleton<MailConfig>(builder.Configuration.GetSection("MailConfig").Get<MailConfig>());
builder.Services.AddSingleton<MailTemplate>(builder.Configuration.GetSection("MailTemplate").Get<MailTemplate>());
//短信配置注册
builder.Services.AddSingleton<MessageConfig>(builder.Configuration.GetSection("MessageConfig").Get<MessageConfig>());
//服务注册
builder.Services.AddScoped<IUserApp,UserApp>();
builder.Services.AddScoped<IMailApp, MailApp>();
builder.Services.AddScoped<IMessageApp, MessageApp>();
builder.Services.AddScoped<IPhotoApp, PhotoApp>();
builder.Services.AddScoped<IFollowApp, FollowApp>();
builder.Services.AddScoped<IScoreApp, ScoreApp>();
builder.Services.AddScoped<IStatisticsApp, StatisticsApp>();
//Redis客户端注册
builder.Services.AddSingleton<IConnectionMultiplexer>(cm =>
{
    var conStr = builder.Configuration.GetValue<string>("RedisServer");
    return ConnectionMultiplexer.Connect(conStr);
});
//RBAC授权服务
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, RBACPolicyProvider>();
builder.Services.AddSingleton<IAuthorizationHandler, RBACRequirementHandler>();

// 数据库连接池注册
builder.Services.AddPooledDbContextFactory<UserDBContext>(option =>
{
    option.UseMySql(builder.Configuration.GetValue<string>("MySQL"),new MySqlServerVersion(new Version(8,0,27)));
});

//认证中心注册
builder.Services.AddAuthentication("Bearer")
.AddJwtBearer("Bearer", options =>
{
    options.Authority = ServiceUrl.GetServiceUrlByName("IdentityService",
        builder.Configuration.GetSection("Consul").Get<ConsulServiceOptions>().ConsulAddress);
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = false
    };
});

//gRPC注册
builder.Services.AddGrpc();
builder.Services.AddGrpcClient<gRole.gRoleClient>(option =>
{
    option.Address = new Uri(ServiceUrl.GetServiceUrlByName("IdentityService", builder.Configuration.GetSection("Consul").Get<ConsulServiceOptions>().ConsulAddress));
});
//Redis客户端注册
builder.Services.AddSingleton<IConnectionMultiplexer>(cm =>
{   var conStr = builder.Configuration.GetValue<string>("RedisServer");
    return ConnectionMultiplexer.Connect(conStr);
});

var app = builder.Build();

//consul服务注册注入
app.UseConsul(builder.Configuration.GetSection("Consul").Get<ConsulServiceOptions>());

//注入rpc服务
app.MapGrpcService<GUserService>();

//节点注册
if (builder.Environment.IsDevelopment())
{
    app.Lifetime.ApplicationStarted.Register(async () =>
    {
        await app.UsePermissionRegistrar<Program>(builder.Configuration.GetSection("Consul").Get<ConsulServiceOptions>().ConsulAddress);
    });
}

//开启静态文件访问
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "static","avatar")),
    RequestPath = "/api/avatar"
});
//开启静态文件访问
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "static", "cover")),
    RequestPath = "/api/cover"
});

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

//启用授权鉴权
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
