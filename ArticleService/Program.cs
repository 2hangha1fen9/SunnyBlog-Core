using ArticleService;
using ArticleService.App;
using ArticleService.App.Interface;
using Com.Ctrip.Framework.Apollo;
using Com.Ctrip.Framework.Apollo.Enums;
using Infrastructure;
using Infrastructure.Auth;
using Infrastructure.Consul;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using StackExchange.Redis;
using Microsoft.AspNetCore.Http;
using System.Reflection;
using ArticleService.Rpc.Service;
using static ArticleService.Rpc.Protos.gUser;
using ArticleService.Rpc.Protos;
using Microsoft.Extensions.FileProviders;
using ArticleService.Domain;
using CommentService.Rpc.Protos;

var builder = WebApplication.CreateBuilder(args);
//Apollo��������
builder.Host.ConfigureAppConfiguration((context, builder) =>
{
    builder.AddApollo(builder.Build()
            .GetSection("Apollo"))
            .AddDefault()
            .AddNamespace("ArticleService", ConfigFileFormat.Json);
});
// Add services to the container.

builder.WebHost.UseUrls("https://*:8082");
builder.Services.AddControllers().AddNewtonsoftJson(option =>
{
    //�������л�ѭ������
    option.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    //���ýӿ�ע��
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

//����ע��
builder.Services.AddScoped<IArticleApp, ArticleApp>();
builder.Services.AddScoped<IArticleCategoryApp, ArticleCategoryApp>();
builder.Services.AddScoped<IArticleRegionApp, ArticleRegionApp>();
builder.Services.AddScoped<IArticleTagApp, ArticleTagApp>();
builder.Services.AddScoped<IDrawingBedApp, DrawingBedApp>();
builder.Services.AddScoped<ISettingApp, SettingApp>();
builder.Services.AddScoped<ISiteConfigApp, SiteConfigApp>();
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
builder.Services.AddPooledDbContextFactory<ArticleDBContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetValue<string>("SqlServer"));
});

//��֤����ע��
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

//gRPCע��
builder.Services.AddGrpc();
builder.Services.AddGrpcClient<gUser.gUserClient>(option =>
{
    option.Address = new Uri(ServiceUrl.GetServiceUrlByName("UserService", builder.Configuration.GetSection("Consul").Get<ConsulServiceOptions>().ConsulAddress));
});
builder.Services.AddGrpcClient<gRank.gRankClient>(option =>
{
    option.Address = new Uri(ServiceUrl.GetServiceUrlByName("CommentService", builder.Configuration.GetSection("Consul").Get<ConsulServiceOptions>().ConsulAddress));
});
builder.Services.AddGrpcClient<gMark.gMarkClient>(option =>
{
    option.Address = new Uri(ServiceUrl.GetServiceUrlByName("CommentService", builder.Configuration.GetSection("Consul").Get<ConsulServiceOptions>().ConsulAddress));
});

var app = builder.Build();

// consulע��
app.UseConsul(builder.Configuration.GetSection("Consul").Get<ConsulServiceOptions>());

//�ڵ�ע��
app.UsePermissionRegistrar<Program>(builder.Configuration.GetSection("Consul").Get<ConsulServiceOptions>().ConsulAddress).Wait();

//rpc����ע��
app.MapGrpcService<GArticleService>();
app.MapGrpcService<GSettingService>();

//������̬�ļ�����
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "static", "picture")),
    RequestPath = "/api/picture"
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//������Ȩ��Ȩ
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
