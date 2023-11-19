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
           Description = "用于内网网络设备发现和唤醒",
           Contact = new OpenApiContact
           {
               Email = "sang93@qq.com",
               Url = new Uri("https://github.com/sangyuxiaowu"),
           },
           Version = "v1"
       }
    );
    //设置xml引用
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
    // 从配置文件读取 Token
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
        return new CallBack(false, 400, "MAC地址格式错误");
    }
    WOL.Send(mac);
    return new CallBack(true, 200, "发送成功");
})
#if DEBUG
.WithOpenApi(operation => new(operation)
{
    Summary = "执行网络唤醒",
    Description = "通过传入MAC地址唤醒局域网内的设备"
})
#else
.WithMetadata(new SignAuthorizeAttribute())
#endif
;


app.MapGet("/devices", () =>
{
    // 从配置文件读取设备列表
    var devices = app.Configuration.GetSection("Devices").Get<List<Device>>();
    // 判断IP是否在线
    Parallel.ForEach(devices, (device) =>
    {
        device.Online = Helper.Ping(device.IP);
    });
    return new CallBack<List<Device>>(true, 200, "获取成功", devices);
})
#if DEBUG
.WithOpenApi(operation => new(operation)
{
    Summary = "获取设备配置列表",
    Description = "获取配置的设备信息"
})
#else
.WithMetadata(new SignAuthorizeAttribute())
#endif
;


app.Run();
