using System.Net;
using System.Net.Sockets;
using System.Text;


Console.WriteLine("Logs from your program will appear here!");

const string response = "+PONG\r\n";

var server = new TcpListener(IPAddress.Any, 6379);
server.Start();
var socket = await server.AcceptSocketAsync();

await Task.Run(async () =>
{
    while (true)
    {
        var buffer = new byte[1024];
        var received = await socket.ReceiveAsync(buffer, SocketFlags.None);
        var msg = Encoding.ASCII.GetString(buffer, 0, received);
        if (msg.Length == 0) break;
    
        await socket.SendAsync(Encoding.UTF8.GetBytes(response), SocketFlags.None);
    }
});



