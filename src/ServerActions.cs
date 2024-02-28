using System.Net;
using System.Net.Sockets;
using System.Text;

namespace codecrafters_redis;

public static class ServerActions
{
    private static IPAddress IpAddress => Dns.GetHostEntry("localhost").AddressList[0];
    
    public static async Task SendPingToMaster()
    {
        var endpoint = new IPEndPoint(IpAddress, ServerSettings.MasterPort);
        using var client = new TcpClient();
        await client.ConnectAsync(endpoint);
        var stream = client.GetStream();
        var message = Resp.ArrayEncode(new List<string>{"Ping"});
        
        await stream.WriteAsync(Encoding.UTF8.GetBytes(message));
        Console.WriteLine("Sent PING to master");
    }

    public static async Task SendPortToMaster()
    {
        var endpoint = new IPEndPoint(IpAddress, ServerSettings.MasterPort);
        using var client = new TcpClient();
        await client.ConnectAsync(endpoint);
        var stream = client.GetStream();
        var message = Resp.ArrayEncode(new List<string>
        {
            "REPLCONF", "listening-port", ServerSettings.Port.ToString()
        });
        await stream.WriteAsync(Encoding.UTF8.GetBytes(message));
        Console.WriteLine("Sent listening port to master");

    }
    
    public static async Task SendCapaToMaster()
    {
        var endpoint = new IPEndPoint(IpAddress, ServerSettings.MasterPort);
        using var client = new TcpClient();
        await client.ConnectAsync(endpoint);
        var stream = client.GetStream();
        var message = Resp.ArrayEncode(new List<string>
        {
            "REPLCONF", "capa", "psync2"
        });
        await stream.WriteAsync(Encoding.UTF8.GetBytes(message));
        Console.WriteLine("Sent Capa  to master");

    }
   
}