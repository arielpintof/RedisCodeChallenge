using System.Net;
using System.Net.Sockets;
using System.Text;

namespace codecrafters_redis;

public static class ServerActions
{
    private static IPAddress IpAddress => Dns.GetHostEntry("localhost").AddressList[0];
    private static IPEndPoint EndPoint => new(IpAddress, ServerSettings.MasterPort);

    public static async void HandShakeToMaster()
    {
        var buffer = new byte[1024];
        using var client = new TcpClient();
        Console.WriteLine($"Sending to Port: {ServerSettings.MasterPort}");
        await client.ConnectAsync(EndPoint);
        
        var stream = client.GetStream();
        await stream.WriteAsync(EncodePing);
        await stream.ReadAsync(buffer);
        Console.WriteLine($"Recibido: {Encoding.UTF8.GetString(buffer)}");
        await stream.WriteAsync(EncodePort);
        await stream.ReadAsync(buffer);
        Console.WriteLine($"Recibido: {Encoding.UTF8.GetString(buffer)}");
        await stream.WriteAsync(EncodeCapa);
        await stream.ReadAsync(buffer);
        Console.WriteLine($"Recibido: {Encoding.UTF8.GetString(buffer)}");
        await stream.WriteAsync(EncodePsync);
        await stream.ReadAsync(buffer);
        Console.WriteLine($"Recibido: {Encoding.UTF8.GetString(buffer)}");
    }
    
    private static byte[] EncodePing => Encoding.UTF8.GetBytes(Resp.ArrayEncode(new List<string>{"Ping"}));

    private static byte[] EncodePort =>  Encoding.UTF8.GetBytes(
        Resp.ArrayEncode(new List<string> { "REPLCONF", "listening-port", ServerSettings.Port.ToString() }));

    private static byte[] EncodeCapa => Encoding.UTF8.GetBytes(
        Resp.ArrayEncode(new List<string> { "REPLCONF", "capa", "psync2" }));

    private static byte[] EncodePsync => Encoding.UTF8.GetBytes(
        Resp.ArrayEncode(new List<string> { "PSYNC", "?", "-1" }));
}