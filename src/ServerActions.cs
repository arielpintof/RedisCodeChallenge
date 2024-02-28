using System.Net;
using System.Net.Sockets;
using System.Text;

namespace codecrafters_redis;

public static class ServerActions
{
    public static async Task SendPingToMaster()
    {
        var ipAddress = (await Dns.GetHostEntryAsync("localhost")).AddressList[0];
        var endpoint = new IPEndPoint(ipAddress, ServerSettings.MasterPort);
        using var client = new TcpClient();
        await client.ConnectAsync(endpoint);
        
        var stream = client.GetStream();
        var message = Resp.ArrayEncode(new List<string>(){"Ping"});
        
        await stream.WriteAsync(Encoding.UTF8.GetBytes(message));
        Console.WriteLine("Sent PING to master");
    }
}