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
using StackExchange.Redis;
using Microsoft.Extensions.Caching.Distributed;

var builder = WebApplication.CreateBuilder(args);
//Apollo��������
builder.Host.ConfigureAppConfiguration((context,builder) =>
{
    builder.AddApollo(builder.Build()
            .GetSection("Apollo"))
            .AddDefault()
            .AddNamespace("UserService",ConfigFileFormat.Json);
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

#region ����ע��
builder.Services.AddScoped<IUserApp,UserApp>();
//����Redis����
builder.Services.AddSingleton(new RedisCache(builder.Configuration.GetValue<string>("RedisServer")));
//RBAC��Ȩ����
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, RBACPolicyProvider>();
builder.Services.AddSingleton<IAuthorizationHandler, RBACRequirementHandler>();
#endregion

// ���ݿ����ӳ�ע��
builder.Services.AddPooledDbContextFactory<UserDBContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetValue<string>("SqlServer"));
});

//Redis�ͻ���ע��
builder.Services.AddStackExchangeRedisCache(option =>
{
    option.InstanceName = "UserService";
    option.Configuration = builder.Configuration.GetValue<string>("RedisServer");
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

var app = builder.Build();


//consul����ע��ע��
app.UseConsul(builder.Configuration.GetSection("Consul").Get<ConsulServiceOptions>());

//ע��rpc����
app.MapGrpcService<GUserService>();

//�ڵ�ע��
app.UsePermissionRegistrar<Program>(builder.Configuration.GetSection("Consul").Get<ConsulServiceOptions>().ConsulAddress);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();



app.Run();
