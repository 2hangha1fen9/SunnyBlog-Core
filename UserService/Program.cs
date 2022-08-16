using ConsulBuilder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using UserService;
using UserService.App;
using UserService.App.Interface;
using Infrastructure.Auth;
using Microsoft.IdentityModel.Tokens;
using UserService.Rpc.Service;

var builder = WebApplication.CreateBuilder(args);

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
//RBAC��Ȩ����
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, RBACPolicyProvider>();
builder.Services.AddSingleton<IAuthorizationHandler, RBACRequirementHandler>();
#endregion

#region ���ݿ�ע��
builder.Services.AddDbContext<UserDBContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
});
#endregion

#region ��֤����ע��
builder.Services.AddAuthentication("Bearer")
.AddJwtBearer("Bearer", options =>
{
    options.Authority = "https://localhost:8000";

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = false
    };
});
#endregion
//gRPCע��
builder.Services.AddGrpc();


var app = builder.Build();

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

#region consulע��
//��ȡappsetings��consul����
var consulSection = builder.Configuration.GetSection("Consul");
//����consul���ö���
var consulOption = new ConsulServiceOptions()
{
    ServiceName = consulSection["ServiceName"],
    ServiceIP = consulSection["ServiceIP"],
    ServicePort = Convert.ToInt32(consulSection["ServicePort"]),
    ServiceHealthCheck = consulSection["ServiceHealthCheck"],
    ConsulAddress = consulSection["ConsulAddress"]
};
app.UseConsul(consulOption);
#endregion

//ע��rpc����
app.MapGrpcService<GUserService>();

app.Run();
