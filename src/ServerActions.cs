﻿using System.Net;
using System.Net.Sockets;
using System.Text;

namespace codecrafters_redis;

public static class ServerActions
{
    private static IPAddress IpAddress => Dns.GetHostEntry("localhost").AddressList[0];
    private static IPEndPoint EndPoint => new(IpAddress, ServerSettings.MasterPort);

    public static async void HandShakeToMaster(TcpClient client)
    {
        var buffer = new byte[1024];
        
        await client.ConnectAsync(EndPoint);
        
        var stream = client.GetStream();
        await stream.WriteAsync(EncodePing);
        var readAsync = await stream.ReadAsync(buffer);
        if (!Encoding.UTF8.GetString(buffer).Contains("PONG")) {
            throw new Exception("Ping Failed");
        }
        await stream.WriteAsync(EncodePort);
        await stream.WriteAsync(EncodeCapa);
        await stream.WriteAsync(EncodePsync);
    }
    
    private static byte[] EncodePing => Encoding.UTF8.GetBytes(Resp.ArrayEncode(new List<string>{"Ping"}));

    private static byte[] EncodePort =>  Encoding.UTF8.GetBytes(
        Resp.ArrayEncode(new List<string> { "REPLCONF", "listening-port", ServerSettings.Port.ToString() }));

    private static byte[] EncodeCapa => Encoding.UTF8.GetBytes(
        Resp.ArrayEncode(new List<string> { "REPLCONF", "capa", "psync2" }));

    private static byte[] EncodePsync => Encoding.UTF8.GetBytes(
        Resp.ArrayEncode(new List<string> { "PSYNC", "?", "-1" }));
}