using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

namespace WakeOnLan
{
    internal class Helper
    {
        internal static bool IsValidMacAddress(string macAddress)
        {
            Regex regex = new Regex("^([0-9a-fA-F]{2}[:-]?){5}([0-9a-fA-F]{2})$");
            return regex.IsMatch(macAddress);
        }

        internal static bool Ping(string iP)
        {
            // 检查IP是否在线
            Ping ping = new Ping();
            PingReply pingReply = ping.Send(iP,100);
            return pingReply.Status == IPStatus.Success;
        }
    }
}
