using System.Text.RegularExpressions;

namespace WakeOnLan
{

    /// <summary>
    /// 返回结果
    /// </summary>
    /// <param name="success">执行结果</param>
    /// <param name="status">状态码</param>
    /// <param name="msg">消息</param>
    internal record CallBack(bool success, int status, string msg);


    /// <summary>
    /// 返回结果
    /// </summary>
    /// <param name="success">执行结果</param>
    /// <param name="status">状态码</param>
    /// <param name="msg">消息</param>
    /// <param name="data">数据</param>
    internal record CallBack<T>(bool success, int status, string msg, T data);

    /// <summary>
    /// 设备信息
    /// </summary>
    /// <param name="Name">备注名</param>
    /// <param name="IP">IP地址</param>
    /// <param name="MAC">MAC地址</param>
    internal record Device(string Name, string IP, string MAC) { 
        /// <summary>
        /// 设备是否在线
        /// </summary>
        public bool Online { get; set; } = false;  
    };



}