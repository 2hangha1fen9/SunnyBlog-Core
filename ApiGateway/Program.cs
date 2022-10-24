using Com.Ctrip.Framework.Apollo;
using Com.Ctrip.Framework.Apollo.Enums;
using Infrastructure.Consul;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Ocelot.Provider.Polly;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

//�������û�ȡ
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
    //Apollo��������
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
   //����Ocelot���ص�ַ ���Ocelot�����ļ�
    builder.Configuration.AddJsonFile("Ocelot.json", optional: false, reloadOnChange: true);
}

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//����https֤��
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


//�������ؿ���
builder.Services.AddCors(option =>
{
    //���Ĭ������
    option.AddPolicy("default", policy =>
     {
         //ǰ�˵�ַ
         policy.SetIsOriginAllowed(s => true) //����Դ��ַ
         .AllowAnyHeader() //��������ͷ����Ϣ
         .AllowAnyMethod();//��������Http����
     });
});
//ע�����.ͬʱע������ֺ���������
builder.Services.AddOcelot(builder.Configuration).AddConsul().AddPolly();

var app = builder.Build();

// consulע��
app.UseConsul(builder.Configuration.GetSection("Consul").Get<ConsulServiceOptions>());

app.UseRouting(); //ʹ��·��
app.UseCors("default"); //ʹ��Ĭ�Ͽ�������

app.UseEndpoints(endpoints => //ӳ�������
{
    endpoints.MapControllers();
});
app.UseOcelot().Wait(); //��������

if (isHttps)
{
    app.UseHttpsRedirection();
}


app.Run();
