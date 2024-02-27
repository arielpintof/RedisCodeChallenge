using System.Net;
using System.Net.Sockets;
using System.Text;
using codecrafters_redis;


public static class Server
{
    public static async Task Main(List<string> args)
    {
        var port = args.Count >= 2 ? int.Parse(args.ElementAt(1)) : 6379;
        var server = new TcpListener(IPAddress.Any, port);

        server.Start();

        while (true)
        {
            var store = new Store();
            var client = await server.AcceptTcpClientAsync();
            Task.Run(async () =>
            {
                var buffer = new byte[1024];
                var stream = client.GetStream();
                var received = stream.Read(buffer, 0, buffer.Length);
                while (received > 0)
                {
                    var data = Encoding.UTF8.GetString(buffer);
                    var expression = Resp.Decode(data);
                    var message = expression.GetMessage(store);
                    await stream.WriteAsync(Encoding.UTF8.GetBytes(message));
                    received = stream.Read(buffer, 0, buffer.Length);
                }
            });
    
        }
    }
}

