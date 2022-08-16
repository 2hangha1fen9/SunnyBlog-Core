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

#region Ocelot����
//����Ocelot���ص�ַ ���Ocelot�����ļ�
builder.Configuration.AddJsonFile("Ocelot.json", optional: false, reloadOnChange: true);
builder.WebHost.UseUrls("https://*:8888");
//�������ؿ���
builder.Services.AddCors(option =>
{
    //���Ĭ������
    option.AddPolicy("default", policy =>
     {
         //ǰ�˵�ַ
         policy.WithOrigins("http://localhost:8080")
         .AllowAnyHeader() //��������ͷ����Ϣ
         .AllowAnyMethod();//��������Http����
     });
});
//ע�����.ͬʱע������ֺ���������
builder.Services.AddOcelot(builder.Configuration).AddConsul().AddPolly();
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

#region consulע��
app.UseRouting(); //ʹ��·��
app.UseCors("default"); //ʹ��Ĭ�Ͽ�������
app.UseOcelot().Wait(); //��������
app.UseEndpoints(endpoints => //ӳ�������
{
    endpoints.MapControllers();
});
#endregion

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
