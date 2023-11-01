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
#endif

var app = builder.Build();

app.UseSignAuthorization(opt => {
    // �������ļ���ȡ Token
    opt.sToken = app.Configuration["Token"];
});

#if DEBUG
app.UseSwagger();
app.UseSwaggerUI();
#endif


app.MapGet("/wol", (string mac) =>
{
    if (!WOL.IsValidMacAddress(mac))
    {
        return new CallBack(false, 400, "MAC��ַ��ʽ����");
    }
    WOL.Send(mac);
    return new CallBack(true, 200, "���ͳɹ�");
})
#if DEBUG
.WithName("WOL")
.WithOpenApi(operation => new(operation)
{
    Summary = "ִ�����绽��",
    Description = "ͨ������MAC��ַ���Ѿ������ڵ��豸"
})
#else
.WithMetadata(new SignAuthorizeAttribute())
#endif
;

app.Run();
