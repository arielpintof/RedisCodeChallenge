using System.Net;
using System.Net.Sockets;
using System.Text;

namespace codecrafters_redis;

public static class ServerSettings
{
    public static int Port { get; set; } = 6379;
    public static string Role { get; set; } = "master";
    public static string MasterHost { get; set; }
    public static int MasterPort { get; set; }
    

    public static void Configure(string[] args)
    {
        var portIndex = Array.IndexOf(args, "--port");
        if (portIndex != -1)
        {
            Port = int.Parse(args[portIndex + 1]);
        }
        
        var roleIndex = Array.IndexOf(args, "--replicaof");
        if (roleIndex != -1)
        {
            Role = "slave";
            MasterHost = args[roleIndex + 1];
            MasterPort = int.Parse(args[roleIndex + 2]);
        }
    }

    public static bool IsMaster() => Role.Equals("master");

    public static async void SendPingToMaster()
    {
        var ipAddress = (await Dns.GetHostEntryAsync("localhost")).AddressList[0];
        var endpoint = new IPEndPoint(ipAddress, MasterPort);
        using var client = new TcpClient();
        await client.ConnectAsync(endpoint);
        
        var stream = client.GetStream();
        var message = Resp.ArrayEncode(new List<string>(){"Ping"});
        
        await stream.WriteAsync(Encoding.UTF8.GetBytes(message));
        Console.WriteLine("Sent PING to master");
    }
    




}

