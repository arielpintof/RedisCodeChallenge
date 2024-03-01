using System.Net;
using System.Net.Sockets;
using System.Text;

namespace codecrafters_redis;

public static class ServerActions
{
    private static IPAddress IpAddress => Dns.GetHostEntry("localhost").AddressList[0];
    public static IPEndPoint EndPoint => new(IpAddress, ServerSettings.MasterPort);

    public static async void HandShakeToMaster()
    {
        var buffer = new byte[1024];
        using var client = new TcpClient();
        Console.WriteLine($"Sending to Port: {ServerSettings.MasterPort}");
        await client.ConnectAsync(EndPoint);
        
        var stream = client.GetStream();
        await stream.WriteAsync(EncodePing);
        await stream.ReadAsync(buffer);
        
        await stream.WriteAsync(EncodePort);
        await stream.ReadAsync(buffer);
       
        await stream.WriteAsync(EncodeCapa);
        await stream.ReadAsync(buffer);
       
        await stream.WriteAsync(EncodePsync);
        await stream.ReadAsync(buffer);
        
    }
    
    
    private static byte[] EncodePing => Resp.ArrayEncode(new List<string>{"Ping"}).AsByte();

    private static byte[] EncodePort => Resp.ArrayEncode(new List<string> { "REPLCONF", "listening-port", ServerSettings.Port.ToString() }).AsByte();

    private static byte[] EncodeCapa => Resp.ArrayEncode(new List<string> { "REPLCONF", "capa", "psync2" }).AsByte();

    private static byte[] EncodePsync => Resp.ArrayEncode(new List<string> { "PSYNC", "?", "-1" }).AsByte();
}

public static class StringExtension
{
    public static byte[] AsByte(this string value) => Encoding.UTF8.GetBytes(value).ToArray();
    
    public static byte[] ToBinary(this string value) => Convert.FromBase64String(value);
    
}