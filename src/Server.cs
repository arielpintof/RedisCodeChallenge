using System.Net;
using System.Net.Sockets;
using System.Text;
using codecrafters_redis;


public static class Server
{
    public static async Task Main(string[] args)
    {
        ServerSettings.Configure(args);
        var server = new TcpListener(IPAddress.Any, ServerSettings.Port);
        
        server.Start();
        
        if (!ServerSettings.IsMaster())
        {
            ServerActions.HandShakeToMaster();
        }
        
        while (true)
        {
            var store = new Store();
            var client = await server.AcceptTcpClientAsync();

            Task.Run(async () =>
            {
                var buffer = new byte[1024];
                var stream = client.GetStream();
                var received = await stream.ReadAsync(buffer);
                while (received > 0)
                {
                    var data = Encoding.UTF8.GetString(buffer);
                    var expression = Resp.Decode(data);
                    var message = expression.GetMessage(store);
                    await stream.WriteAsync(Encoding.UTF8.GetBytes(message));
                    received = await stream.ReadAsync(buffer);
                }

            });

        }
    }
}

