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
    //���ýӿ�ע��
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

//�������ݿ�
builder.Services.AddDbContext<AuthDBContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
});
//��ӷ���
builder.Services.AddScoped<IPermissionApp, PermissionApp>();

//������Ȩ���񣬲��Զ�������token
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

//ע����Ȩ����Ids4
app.UseIdentityServer();

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
app.Run();
#endregion