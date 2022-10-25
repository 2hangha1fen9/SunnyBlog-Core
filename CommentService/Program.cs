using ArticleService.Rpc.Protos;
using Com.Ctrip.Framework.Apollo;
using Com.Ctrip.Framework.Apollo.Enums;
using CommentService;
using CommentService.App;
using CommentService.App.Interface;
using CommentService.Rpc.Protos;
using CommentService.Rpc.Service;
using Infrastructure;
using Infrastructure.Auth;
using Infrastructure.Consul;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);
var useApollo = builder.Configuration.GetValue<bool>("useApollo");
var useSkywalking = builder.Configuration.GetValue<bool>("useSkywalking");
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
                .AddNamespace("CommentService", ConfigFileFormat.Json);
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

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    //���ýӿ�ע��
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});
//�������ݿ�
builder.Services.AddDbContextFactory<CommentDBContext>(option =>
{
    // option.UseSqlServer(builder.Configuration.GetValue<string>("SqlServer"));
    option.UseMySql(builder.Configuration.GetValue<string>("MySQL"), new MySqlServerVersion(new Version(8, 0, 27)));
});
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
//��֤����ע��
builder.Services.AddAuthentication("Bearer")
.AddJwtBearer("Bearer", options =>
{
    options.Authority = ServiceUrl.GetServiceUrlByName("IdentityService",
        builder.Configuration.GetSection("Consul").Get<ConsulServiceOptions>().ConsulAddress, false);
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = false
    };
    options.RequireHttpsMetadata = isHttps;
});


//gRPC�ͻ���ע��
builder.Services.AddGrpc();
builder.Services.AddGrpcClient<gArticle.gArticleClient>(option =>
{
    option.Address = new Uri(ServiceUrl.GetServiceUrlByName("ArticleService", builder.Configuration.GetSection("Consul").Get<ConsulServiceOptions>().ConsulAddress));
});
builder.Services.AddGrpcClient<gUser.gUserClient>(option =>
{
    option.Address = new Uri(ServiceUrl.GetServiceUrlByName("UserService", builder.Configuration.GetSection("Consul").Get<ConsulServiceOptions>().ConsulAddress));
});
builder.Services.AddGrpcClient<gSetting.gSettingClient>(option =>
{
    option.Address = new Uri(ServiceUrl.GetServiceUrlByName("ArticleService", builder.Configuration.GetSection("Consul").Get<ConsulServiceOptions>().ConsulAddress));
});

//����ע��
builder.Services.AddScoped<ICommentApp, CommentApp>();
builder.Services.AddScoped<ILikeApp, LikeApp>();
builder.Services.AddScoped<IViewApp, ViewApp>();
builder.Services.AddScoped<IMetaApp, MetaApp>();
builder.Services.AddScoped<IDrawingBedApp, DrawingBedApp>();
builder.Services.AddScoped<IStatisticsApp, StatisticsApp>();
//Redis�ͻ���ע��
builder.Services.AddSingleton<IConnectionMultiplexer>(cm =>
{
    var conStr = builder.Configuration.GetValue<string>("RedisServer");
    return ConnectionMultiplexer.Connect(conStr);
});

var app = builder.Build();

// consulע��
app.UseConsul(builder.Configuration.GetSection("Consul").Get<ConsulServiceOptions>());

//�ڵ�ע��
app.Lifetime.ApplicationStarted.Register(async () =>
{
    await app.UsePermissionRegistrar<Program>(builder.Configuration.GetSection("Consul").Get<ConsulServiceOptions>().ConsulAddress);
});

//������̬�ļ�����
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "static", "picture")),
    RequestPath = "/api/picture"
});

//rpc����ע��
app.MapGrpcService<GRankService>();
app.MapGrpcService<GMarkService>();

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
