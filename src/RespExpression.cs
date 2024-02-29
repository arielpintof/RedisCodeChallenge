

namespace codecrafters_redis;

public class RespExpression
{
    public readonly List<string> Value;

    public RespExpression(IEnumerable<string> value)
    {
        Value = value.ToList();
    }

    private Command Command => Value[2] switch
    {
        "ping" => Command.Ping,
        "echo" => Command.Echo,
        "set" => Command.Set,
        "get" => Command.Get,
        "info" => Command.Info,
        "replconf" => Command.Replconf,
        "psync" => Command.Psync,
        _ => throw new ArgumentOutOfRangeException()
    };
    
    private CommandOption CommandOption => Value[8] switch
    {
        "px" => CommandOption.Px,
        _ => throw new ArgumentOutOfRangeException()
    };

    public string GetMessage(Store store) => Command switch
    {
        Command.Ping => HandlePingCommand(),
        Command.Echo => Resp.BulkEncode(Value[4]),
        Command.Set => HandleSetCommand(store),
        Command.Get => HandleGetCommand(store),
        Command.Info => HandleInfoCommand(),
        Command.Replconf => HandleReplCommand(),
        _ => throw new ArgumentOutOfRangeException()
    };

    private string HandleReplCommand()
    {
        Console.WriteLine($"Recibiendo RESPLCONF...");
        return Resp.SimpleEncode("OK");
    }

    private string HandlePingCommand()
    {
        Console.WriteLine($"Recibiendo PING...");
        return Resp.SimpleEncode("PONG");
    }

    private string HandleInfoCommand()
    {
        if (!this.ValueHas("info") && !this.ValueHas("replication"))
            return Resp.BulkEncode($"role:{ServerSettings.Role}");
        
        var response = new List<string>
        {
            $"role:{ServerSettings.Role}",
            "master_replid:8371b4fb1155b71f4a04d3e1bc3e18c4a990aeeb",
            "master_repl_offset:0"
        };
            
        return Resp.BulkEncode(string.Join(Resp.Separator, response));
    }

   
    private string HandleSetCommand(Store store)
    {
        var expiration = TryGetArgument("px", out var expirationValue) 
            ? int.Parse(expirationValue)
            : 0;
        
        return store.Set(GetKey, GetValue, expiration) ? Resp.SimpleEncode("OK") : "null";
    }

    private string HandleGetCommand(Store store)
    {
        var value = store.GetValue(GetKey);
        return value!.Length > 0 ? Resp.BulkEncode(value) : Resp.NullEncode();
    }
    
    private string GetKey => Value[4];
    private string GetValue => Value[6];

    private bool TryGetArgument(string key, out string value)
    {
        var index = Value.FindIndex(e => e == key);
        if (index is -1)
        {
            value = string.Empty;
            return false;
        }

        value = $"{index}";
        return true;
    }

}

public static class RespExtension
{
    public static bool ValueHas(this RespExpression expression, string argument)
    {
        return expression.Value.Any(e => e.Equals(argument));
    }

    
    
}