using Com.Ctrip.Framework.Apollo;
using Com.Ctrip.Framework.Apollo.Enums;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Ocelot.Provider.Polly;

var builder = WebApplication.CreateBuilder(args);
//Apollo��������
builder.Host.ConfigureAppConfiguration((context, builder) =>
{
    builder.AddApollo(builder.Build()
            .GetSection("Apollo"))
            .AddDefault()
            .AddNamespace("ApiGateway", ConfigFileFormat.Json);
});
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//����Ocelot���ص�ַ ���Ocelot�����ļ�
//builder.Configuration.AddJsonFile("Ocelot.json", optional: false, reloadOnChange: true);
builder.WebHost.UseUrls("https://*:8888");
//�������ؿ���
builder.Services.AddCors(option =>
{
    //���Ĭ������
    option.AddPolicy("default", policy =>
     {
         //ǰ�˵�ַ
         policy.WithOrigins(builder.Configuration.GetValue<string>("WebClient"))
         .AllowAnyHeader() //��������ͷ����Ϣ
         .AllowAnyMethod();//��������Http����
     });
});
//ע�����.ͬʱע������ֺ���������
builder.Services.AddOcelot(builder.Configuration).AddConsul().AddPolly();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting(); //ʹ��·��
app.UseCors("default"); //ʹ��Ĭ�Ͽ�������
app.UseOcelot().Wait(); //��������
app.UseEndpoints(endpoints => //ӳ�������
{
    endpoints.MapControllers();
});

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
