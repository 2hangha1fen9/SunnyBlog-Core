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

var builder = WebApplication.CreateBuilder(args);
//Apollo配置中心
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
    //配置接口注释
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

//配置数据库
builder.Services.AddDbContextFactory<AuthDBContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetValue<string>("SqlServer"));
});

//注册gRPC
builder.Services.AddGrpc();

//添加服务
builder.Services.AddScoped<IPermissionApp, PermissionApp>();

//启用授权服务，并自定义令牌token
builder.Services.AddIdentityServer()
    .AddInMemoryIdentityResources(Config.IdentityResources())
    .AddInMemoryClients(Config.Clients())
    .AddResourceOwnerValidator<CustomResourceOwnerPasswordValidator>()
    .AddProfileService<CustomProfileService>()
    .AddDeveloperSigningCredential();

// Configure the HTTP request pipeline.
var app = builder.Build();

//consul注入
app.UseConsul(builder.Configuration.GetSection("Consul").Get<ConsulServiceOptions>());

//gRPC服务映射
app.MapGrpcService<GEndpointService>();

//注入授权服务Ids4
app.UseIdentityServer();

//节点注册
app.UsePermissionRegistrar<Program>(builder.Configuration.GetSection("Consul").Get<ConsulServiceOptions>().ConsulAddress);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
