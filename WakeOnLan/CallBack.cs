namespace WakeOnLan
{

    /// <summary>
    /// 返回结果
    /// </summary>
    /// <param name="success">执行结果</param>
    /// <param name="status">状态码</param>
    /// <param name="msg">消息</param>
    internal record CallBack(bool success, int status, string msg);

}