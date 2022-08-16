using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Ocelot.Provider.Polly;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region Ocelot配置
//配置Ocelot网关地址 添加Ocelot配置文件
builder.Configuration.AddJsonFile("Ocelot.json", optional: false, reloadOnChange: true);
builder.WebHost.UseUrls("https://*:8888");
//配置网关跨域
builder.Services.AddCors(option =>
{
    //添加默认配置
    option.AddPolicy("default", policy =>
     {
         //前端地址
         policy.WithOrigins("http://localhost:8080")
         .AllowAnyHeader() //允许任意头部信息
         .AllowAnyMethod();//允许任意Http动词
     });
});
//注册服务.同时注册服务发现和流量控制
builder.Services.AddOcelot(builder.Configuration).AddConsul().AddPolly();
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

#region consul注入
app.UseRouting(); //使用路由
app.UseCors("default"); //使用默认跨域配置
app.UseOcelot().Wait(); //启用网关
app.UseEndpoints(endpoints => //映射控制器
{
    endpoints.MapControllers();
});
#endregion

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
