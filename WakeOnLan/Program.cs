#if DEBUG
using Microsoft.OpenApi.Models;
using Sang.AspNetCore.SignAuthorization;
using System.Net.Sockets;
using System.Net;
using System.Text.RegularExpressions;
using System.Net.Mail;
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
#endif

var app = builder.Build();

app.UseSignAuthorization(opt => {
    // 从配置文件读取 Token
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
        return new CallBack(false, 400, "MAC地址格式错误");
    }
    WOL.Send(mac);
    return new CallBack(true, 200, "发送成功");
})
#if DEBUG
.WithName("WOL")
.WithOpenApi()
#endif
;

app.Run();


internal class WOL
{
    internal static void Send(string macAddress)
    {
        byte[] magicPacket = CreateMagicPacket(macAddress);
        SendMagicPacket(magicPacket);
    }

    internal static bool IsValidMacAddress(string macAddress)
    {
        Regex regex = new Regex("^([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})$");
        return regex.IsMatch(macAddress);
    }

    static byte[] CreateMagicPacket(string macAddress)
    {
        byte[] macBytes = ParseMacAddress(macAddress);
        byte[] magicPacket = new byte[6 + (6 * 16)];

        for (int i = 0; i < 6; i++)
        {
            magicPacket[i] = 0xFF;
        }

        for (int i = 6; i < magicPacket.Length; i += 6)
        {
            Array.Copy(macBytes, 0, magicPacket, i, 6);
        }

        return magicPacket;
    }

    static byte[] ParseMacAddress(string macAddress)
    {
        string[] hexValues = macAddress.Split(new[] { ':', '-' });
        byte[] macBytes = new byte[6];

        for (int i = 0; i < hexValues.Length; i++)
        {
            macBytes[i] = Convert.ToByte(hexValues[i], 16);
        }

        return macBytes;
    }

    static void SendMagicPacket(byte[] magicPacket)
    {
        using (UdpClient udpClient = new UdpClient())
        {
            udpClient.Connect(IPAddress.Broadcast, 9);
            udpClient.Send(magicPacket, magicPacket.Length);
        }
    }

}


/// <summary>
/// 返回结果
/// </summary>
/// <param name="success">执行结果</param>
/// <param name="status">状态码</param>
/// <param name="msg">消息</param>
internal record CallBack(bool success, int status, string msg);