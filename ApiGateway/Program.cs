using Com.Ctrip.Framework.Apollo;
using Com.Ctrip.Framework.Apollo.Enums;
using Infrastructure.Consul;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Ocelot.Provider.Polly;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

//基础配置获取
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
    //Apollo配置中心
    builder.Host.ConfigureAppConfiguration((context, builder) =>
    {
        builder.AddApollo(builder.Build()
                .GetSection("Apollo"))
                .AddDefault()
                .AddNamespace("ApiGateway", ConfigFileFormat.Json);
    });
}
else
{
   //配置Ocelot网关地址 添加Ocelot配置文件
    builder.Configuration.AddJsonFile("Ocelot.json", optional: false, reloadOnChange: true);
}

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//配置https证书
if (isHttps)
{   
    builder.WebHost.UseKestrel(option =>
    {
        option.ConfigureHttpsDefaults(c =>
        {
            c.ServerCertificate = new X509Certificate2(httpsCertPath, httpsCertPwd);
        });
    });
    builder.WebHost.UseUrls($"https://*:{port}");
}
else
{
    builder.WebHost.UseUrls($"http://*:{port}");
}


//配置网关跨域
builder.Services.AddCors(option =>
{
    //添加默认配置
    option.AddPolicy("default", policy =>
     {
         //前端地址
         policy.SetIsOriginAllowed(s => true) //任意源地址
         .AllowAnyHeader() //允许任意头部信息
         .AllowAnyMethod();//允许任意Http动词
     });
});
//注册服务.同时注册服务发现和流量控制
builder.Services.AddOcelot(builder.Configuration).AddConsul().AddPolly();

var app = builder.Build();

// consul注入
app.UseConsul(builder.Configuration.GetSection("Consul").Get<ConsulServiceOptions>());

app.UseRouting(); //使用路由
app.UseCors("default"); //使用默认跨域配置

app.UseEndpoints(endpoints => //映射控制器
{
    endpoints.MapControllers();
});
app.UseOcelot().Wait(); //启用网关

if (isHttps)
{
    app.UseHttpsRedirection();
}


app.Run();
