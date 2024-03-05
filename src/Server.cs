using System.Net;
using System.Net.Sockets;
using System.Text;
using codecrafters_redis;


await ServerSettings.Configure(args);

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
    
    new Thread(() => HandleClient(client, store)).Start();
}

async void HandleClient(TcpClient client, Store store)
{
    var buffer = new byte[1024];
    var stream = client.GetStream();
    var received = await stream.ReadAsync(buffer);

    while (received > 0)
    {
        var data = Encoding.UTF8.GetString(buffer);
        var expression = Resp.Decode(data);
        var messages = expression.GetMessage(store);
        foreach (var message in messages)
        {
            await stream.WriteAsync(message);
        }

        received = stream.Read(buffer);
    }
}