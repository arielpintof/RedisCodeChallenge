using System.Net;
using System.Net.Sockets;
using System.Text;
using codecrafters_redis;


var server = new TcpListener(IPAddress.Any, 6379);
server.Start();

var store = new Store();
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
            var data = Encoding.UTF8.GetString(buffer);
            var expression = Resp.Decode(data, store);
            var message = expression.GetMessage();
            await stream.WriteAsync(Encoding.UTF8.GetBytes(message));
            received = stream.Read(buffer, 0, buffer.Length);
        }
    });
    
}
