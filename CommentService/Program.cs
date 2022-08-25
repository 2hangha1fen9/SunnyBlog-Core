using Com.Ctrip.Framework.Apollo;
using Com.Ctrip.Framework.Apollo.Enums;
using Infrastructure;
using Infrastructure.Auth;
using Infrastructure.Consul;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
//Apollo��������
builder.Host.ConfigureAppConfiguration((context, builder) =>
{
    builder.AddApollo(builder.Build()
            .GetSection("Apollo"))
            .AddDefault()
            .AddNamespace("CommentService", ConfigFileFormat.Json);
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
/*//�������ݿ�
builder.Services.AddDbContextFactory<AuthDBContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetValue<string>("SqlServer"));
});*/
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
        builder.Configuration.GetSection("Consul").Get<ConsulServiceOptions>().ConsulAddress);
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = false
    };
});
var app = builder.Build();

// consulע��
app.UseConsul(builder.Configuration.GetSection("Consul").Get<ConsulServiceOptions>());

//�ڵ�ע��
app.UsePermissionRegistrar<Program>(builder.Configuration.GetSection("Consul").Get<ConsulServiceOptions>().ConsulAddress);

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
