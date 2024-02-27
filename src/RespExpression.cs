using System.Collections.Immutable;

namespace codecrafters_redis;

public class RespExpression
{
    private readonly ImmutableArray<string> _value;

    public RespExpression(IEnumerable<string> value)
    {
        _value = value.ToImmutableArray();
    }

    private Command Command => _value[2] switch
    {
        "ping" => Command.Ping,
        "echo" => Command.Echo,
        "set" => Command.Set,
        "get" => Command.Get,
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
        _ => throw new ArgumentOutOfRangeException()
    };

    private string HandleSetCommand(Store store)
    {
        return store.Set(Key, Value) ? Resp.SimpleEncode("OK") : "null";
    }

    private async Task ExpireSetCommand(Store store)
    {
        if (CommandOption == CommandOption.Px)
        {
            await Task.Delay(int.Parse(_value[10]));
            store!.Remove(Key);
        }
    }

    private string HandleGetCommand(Store store)
    {
        var value = store.GetValue(Key);
        return value.Length > 0 ? Resp.BulkEncode(value) : Resp.NullEncode();
    }
    
    private string Key => _value[4];
    private string Value => _value[6];
    
    

    



}