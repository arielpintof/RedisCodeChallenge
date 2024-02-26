using System.Net;
using System.Net.Sockets;
using System.Text;
using codecrafters_redis;


Console.WriteLine("Logs from your program will appear here!");

//var response = Encoding.UTF8.GetBytes("+PONG\r\n");

var server = new TcpListener(IPAddress.Any, 6379);
server.Start();


while (true)
{
    var client = await server.AcceptTcpClientAsync();

    Task.Run(async () =>
    {
        var buffer = new byte[1024];
        var stream = client.GetStream();
        var received = stream.Read(buffer, 0, buffer.Length);
        while (received > 0)
        {
            var _ = Encoding.UTF8.GetString(buffer);
            var expression = Utils.RespDecode(_);
            var message = expression.GetMessage();
            await stream.WriteAsync(Encoding.UTF8.GetBytes(message));
            received = stream.Read(buffer, 0, buffer.Length);
            
        }
        
        
    });
}