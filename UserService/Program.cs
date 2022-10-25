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
using System.Security.Cryptography.X509Certificates;
using System.Net;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);
var useSkywalking = builder.Configuration.GetValue<bool>("useSkywalking");
var useApollo = builder.Configuration.GetValue<bool>("useApollo");
var port = builder.Configuration.GetValue<int>("port");
var httpsCertPath = builder.Configuration.GetValue<string>("httpsCertPath");
var httpsCertPwd = builder.Configuration.GetValue<string>("httpsCertPwd");
var isHttps = !string.IsNullOrWhiteSpace(httpsCertPath);

if (useSkywalking)
{
    builder.Services.AddSkyAPM();
}
if (useApollo)
{
    //Apollo��������
    builder.Host.ConfigureAppConfiguration((context, builder) =>
    {
        builder.AddApollo(builder.Build()
                .GetSection("Apollo"))
                .AddDefault()
                .AddNamespace("UserService", ConfigFileFormat.Json);
    });
}

//����https֤��
if (isHttps)
{
    var cert = new X509Certificate2(httpsCertPath, httpsCertPwd);
    builder.WebHost.UseKestrel(option =>
    {
        //Grpcר��ͨ��
        option.Listen(IPAddress.Any, port + 10000, option =>
        {
            option.Protocols = HttpProtocols.Http2;
            option.UseHttps(option =>
            {
                option.ServerCertificate = cert;
            });
        });
        option.Listen(IPAddress.Any, port, option =>
        {
            option.UseHttps(option =>
            {
                option.ServerCertificate = cert;
            });
        });
    });
}
else
{
    builder.WebHost.UseKestrel(option =>
    {
        //Grpcר��ͨ��
        option.Listen(IPAddress.Any, port + 10000, option =>
        {
            option.Protocols = HttpProtocols.Http2;
        });
        option.Listen(IPAddress.Any, port);
    });
}

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    //���ýӿ�ע��
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

//�ʼ�����ע��
builder.Services.AddSingleton<MailConfig>(builder.Configuration.GetSection("MailConfig").Get<MailConfig>());
builder.Services.AddSingleton<MailTemplate>(builder.Configuration.GetSection("MailTemplate").Get<MailTemplate>());
//��������ע��
builder.Services.AddSingleton<MessageConfig>(builder.Configuration.GetSection("MessageConfig").Get<MessageConfig>());
//����ע��
builder.Services.AddScoped<IUserApp,UserApp>();
builder.Services.AddScoped<IMailApp, MailApp>();
builder.Services.AddScoped<IMessageApp, MessageApp>();
builder.Services.AddScoped<IPhotoApp, PhotoApp>();
builder.Services.AddScoped<IFollowApp, FollowApp>();
builder.Services.AddScoped<IScoreApp, ScoreApp>();
builder.Services.AddScoped<IStatisticsApp, StatisticsApp>();
//Redis�ͻ���ע��
builder.Services.AddSingleton<IConnectionMultiplexer>(cm =>
{
    var conStr = builder.Configuration.GetValue<string>("RedisServer");
    return ConnectionMultiplexer.Connect(conStr);
});
//RBAC��Ȩ����
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, RBACPolicyProvider>();
builder.Services.AddSingleton<IAuthorizationHandler, RBACRequirementHandler>();

// ���ݿ����ӳ�ע��
builder.Services.AddPooledDbContextFactory<UserDBContext>(option =>
{
    //option.UseSqlServer(builder.Configuration.GetValue<string>("SqlServer"));
    option.UseMySql(builder.Configuration.GetValue<string>("MySQL"),new MySqlServerVersion(new Version(8,0,27)));
});

//��֤����ע��
builder.Services.AddAuthentication("Bearer")
.AddJwtBearer("Bearer", options =>
{
    options.Authority = ServiceUrl.GetServiceUrlByName("IdentityService",
        builder.Configuration.GetSection("Consul").Get<ConsulServiceOptions>().ConsulAddress,false);
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = false
    };
    options.RequireHttpsMetadata = isHttps;
});

//gRPCע��
builder.Services.AddGrpc();
builder.Services.AddGrpcClient<gRole.gRoleClient>(option =>
{
    option.Address = new Uri(ServiceUrl.GetServiceUrlByName("IdentityService", builder.Configuration.GetSection("Consul").Get<ConsulServiceOptions>().ConsulAddress));
});
//Redis�ͻ���ע��
builder.Services.AddSingleton<IConnectionMultiplexer>(cm =>
{   var conStr = builder.Configuration.GetValue<string>("RedisServer");
    return ConnectionMultiplexer.Connect(conStr);
});

var app = builder.Build();

//consul����ע��ע��
app.UseConsul(builder.Configuration.GetSection("Consul").Get<ConsulServiceOptions>());

//ע��rpc����
app.MapGrpcService<GUserService>();

//�ڵ�ע��
app.Lifetime.ApplicationStarted.Register(async () =>
{
    await app.UsePermissionRegistrar<Program>(builder.Configuration.GetSection("Consul").Get<ConsulServiceOptions>().ConsulAddress);
});

//������̬�ļ�����
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "static","avatar")),
    RequestPath = "/api/avatar"
});
//������̬�ļ�����
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "static", "cover")),
    RequestPath = "/api/cover"
});

app.UseSwagger();
app.UseSwaggerUI();

if (isHttps)
{
    app.UseHttpsRedirection();
}

//������Ȩ��Ȩ
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
