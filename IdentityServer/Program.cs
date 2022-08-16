using Service.IdentityService;
using Service.IdentityService.App;
using Service.IdentityService.App.Interface;
using Service.IdentityService.Custom;
using Microsoft.EntityFrameworkCore;
using IdentityServer;
using ConsulBuilder;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddDbContext<AuthDBContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
});
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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//注入授权服务Ids4
app.UseIdentityServer();

#region consul注入
//获取appsetings的consul配置
var consulSection = builder.Configuration.GetSection("Consul");
//创建consul配置对象
var consulOption = new ConsulServiceOptions()
{
    ServiceName = consulSection["ServiceName"],
    ServiceIP = consulSection["ServiceIP"],
    ServicePort = Convert.ToInt32(consulSection["ServicePort"]),
    ServiceHealthCheck = consulSection["ServiceHealthCheck"],
    ConsulAddress = consulSection["ConsulAddress"]
};
app.UseConsul(consulOption);
app.Run();
#endregion