using Sang.AspNetCore.SignAuthorization;
using WakeOnLan;


#if DEBUG
using Microsoft.OpenApi.Models;
#endif

var builder = WebApplication.CreateBuilder(args);

#if DEBUG
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1",
       new OpenApiInfo
       {
           Title = "WakeOnLan",
           Description = "�������������豸���ֺͻ���",
           Contact = new OpenApiContact
           {
               Email = "sang93@qq.com",
               Url = new Uri("https://github.com/sangyuxiaowu"),
           },
           Version = "v1"
       }
    );
    //����xml����
    var filePath = Path.Combine(System.AppContext.BaseDirectory, "WakeOnLan.xml");
    c.IncludeXmlComments(filePath);
});

var WOLAllowSpecificOrigins = "_wolAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: WOLAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("*");
                      });
});
#endif

var app = builder.Build();

app.UseSignAuthorization(opt => {
    // �������ļ���ȡ Token
    opt.sToken = app.Configuration["Token"];
});

#if DEBUG
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors(WOLAllowSpecificOrigins);
#endif

app.UseStaticFiles();

app.MapGet("/wol", (string mac) =>
{
    if (!Helper.IsValidMacAddress(mac))
    {
        return new CallBack(false, 400, "MAC��ַ��ʽ����");
    }
    WOL.Send(mac);
    return new CallBack(true, 200, "���ͳɹ�");
})
#if DEBUG
.WithOpenApi(operation => new(operation)
{
    Summary = "ִ�����绽��",
    Description = "ͨ������MAC��ַ���Ѿ������ڵ��豸"
})
#else
.WithMetadata(new SignAuthorizeAttribute())
#endif
;


app.MapGet("/devices", () =>
{
    // �������ļ���ȡ�豸�б�
    var devices = app.Configuration.GetSection("Devices").Get<List<Device>>();
    // �ж�IP�Ƿ�����
    Parallel.ForEach(devices, (device) =>
    {
        device.Online = Helper.Ping(device.IP);
    });
    return new CallBack<List<Device>>(true, 200, "��ȡ�ɹ�", devices);
})
#if DEBUG
.WithOpenApi(operation => new(operation)
{
    Summary = "��ȡ�豸�����б�",
    Description = "��ȡ���õ��豸��Ϣ"
})
#else
.WithMetadata(new SignAuthorizeAttribute())
#endif
;


app.Run();
