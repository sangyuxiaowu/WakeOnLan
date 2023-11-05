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
            string cleanedMacAddress = macAddress.Replace(":", "").Replace("-", "");

            if (cleanedMacAddress.Length != 12)
            {
                throw new ArgumentException("Invalid MAC address format");
            }

            byte[] macBytes = new byte[6];

            for (int i = 0; i < 6; i++)
            {
                macBytes[i] = Convert.ToByte(cleanedMacAddress.Substring(i * 2, 2), 16);
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