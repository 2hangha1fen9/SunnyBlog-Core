using Service.IdentityService;
using Service.IdentityService.App;
using Service.IdentityService.App.Interface;
using Service.IdentityService.Custom;
using Microsoft.EntityFrameworkCore;
using IdentityServer;
using Infrastructure.Consul;
using System.Reflection;
using Com.Ctrip.Framework.Apollo;
using Com.Ctrip.Framework.Apollo.Enums;
using IdentityService.Rpc.Service;
using Infrastructure;
using IdentityService;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using Infrastructure.Auth;
using IdentityService.App.Interface;
using IdentityService.App;
using StackExchange.Redis;
using IdentityService.Rpc.Protos;

var builder = WebApplication.CreateBuilder(args);
//Apollo��������
builder.Host.ConfigureAppConfiguration((context, builder) =>
{
    builder.AddApollo(builder.Build()
        .GetSection("Apollo"))
        .AddDefault()
        .AddNamespace("IdentityService", ConfigFileFormat.Json);
});


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

//�������ݿ�
builder.Services.AddDbContextFactory<AuthDBContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetValue<string>("SqlServer"));
});
//Redis�ͻ���ע��
builder.Services.AddSingleton<IConnectionMultiplexer>(cm =>
{
    var conStr = builder.Configuration.GetValue<string>("RedisServer");
    return ConnectionMultiplexer.Connect(conStr);
});

//ע��gRPC
builder.Services.AddGrpc();
builder.Services.AddGrpcClient<gUser.gUserClient>(option =>
{
    option.Address = new Uri(ServiceUrl.GetServiceUrlByName("UserService", builder.Configuration.GetSection("Consul").Get<ConsulServiceOptions>().ConsulAddress));
});

//��ӷ���
builder.Services.AddScoped<IPermissionApp, PermissionApp>();
builder.Services.AddScoped<IRoleApp,RoleApp>();
builder.Services.AddScoped<IRelationApp,RelationApp>();

//RBAC��Ȩ����
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, RBACPolicyProvider>();
builder.Services.AddSingleton<IAuthorizationHandler, RBACRequirementHandler>();

//������Ȩ���񣬲��Զ�������token
builder.Services.AddIdentityServer()
    .AddInMemoryIdentityResources(Config.IdentityResources())
    .AddInMemoryClients(Config.Clients())
    .AddResourceOwnerValidator<CustomResourceOwnerPasswordValidator>()
    .AddProfileService<CustomProfileService>()
    .AddDeveloperSigningCredential();

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

// Configure the HTTP request pipeline.
var app = builder.Build();

//consulע��
app.UseConsul(builder.Configuration.GetSection("Consul").Get<ConsulServiceOptions>());

//gRPC����ӳ��
app.MapGrpcService<GEndpointService>();

//ע����Ȩ����Ids4
app.UseIdentityServer();

//�ڵ�ע��
app.UsePermissionRegistrar<Program>(builder.Configuration.GetSection("Consul").Get<ConsulServiceOptions>().ConsulAddress);

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
