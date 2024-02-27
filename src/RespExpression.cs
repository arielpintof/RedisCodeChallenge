using System.Collections.Immutable;

namespace codecrafters_redis;

public class RespExpression
{
    private readonly List<string> _value;

    public RespExpression(IEnumerable<string> value)
    {
        _value = value.ToList();
    }

    private Command Command => _value[2] switch
    {
        "ping" => Command.Ping,
        "echo" => Command.Echo,
        "set" => Command.Set,
        "get" => Command.Get,
        "info" => Command.Info,
        _ => throw new ArgumentOutOfRangeException()
    };
    
    private CommandOption CommandOption => _value[8] switch
    {
        "px" => CommandOption.Px,
        _ => throw new ArgumentOutOfRangeException()
    };

    public string GetMessage(Store store) => Command switch
    {
        Command.Ping => Resp.SimpleEncode("PONG"),
        Command.Echo => Resp.BulkEncode(_value[4]),
        Command.Set => HandleSetCommand(store),
        Command.Get => HandleGetCommand(store),
        Command.Info => HandleInfoCommand(),
        _ => throw new ArgumentOutOfRangeException()
    };

    private string HandleInfoCommand()
    {
        return _value[4].Equals("replication") ? "role:master" : Resp.NullEncode();
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
    
    private string GetKey => _value[4];
    private string GetValue => _value[6];

    private bool TryGetArgument(string key, out string value)
    {
        var index = _value.FindIndex(e => e == key);
        if (index is -1)
        {
            value = string.Empty;
            return false;
        }

        value = $"{index}";
        return true;
    }

}