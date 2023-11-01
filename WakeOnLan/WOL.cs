using System.Net.Sockets;
using System.Net;
using System.Text.RegularExpressions;

namespace WakeOnLan
{
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
}