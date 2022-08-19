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

//ע��gRPC
builder.Services.AddGrpc();

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

app.UseAuthorization();

app.MapControllers();

app.Run();
