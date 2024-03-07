using System.Net;
using System.Net.Sockets;

namespace codecrafters_redis;

public static class Propagation
{
    private static Queue<string> _commands = new();
    
    static IPAddress IpAddress => Dns.GetHostEntry("localhost").AddressList[0];

    public static async Task Propagate()
    {
        foreach (var replica in ServerSettings.Replicas)
        {
            var endPoint = new IPEndPoint(IpAddress,int.Parse(replica));
            using var client = new TcpClient();
            await client.ConnectAsync(endPoint);
            await SendCommands(client);
        }
    }

    private static async Task SendCommands(TcpClient client)
    {
        foreach (var command in _commands)
        {
            var stream = client.GetStream();
            await stream.WriteAsync(command.AsByte());
        }
    }

    public static void AddCommand(string command) => _commands.Enqueue(command);


}