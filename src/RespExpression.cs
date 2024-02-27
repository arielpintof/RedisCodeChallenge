using System.Collections.Immutable;

namespace codecrafters_redis;

public class RespExpression
{
    private readonly ImmutableArray<string> _value;
    private readonly Store? _store;

    public RespExpression(IEnumerable<string> value, Store store)
    {
        _value = value.ToImmutableArray();
        _store = store;
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

    public string GetMessage() => Command switch
    {
        Command.Ping => Resp.SimpleEncode("PONG"),
        Command.Echo => Resp.BulkEncode(_value[4]),
        Command.Set => HandleSetCommand(),
        Command.Get => HandleGetCommand(),
        _ => throw new ArgumentOutOfRangeException()
    };

    private string HandleSetCommand()
    {
        return _store!.Set(Key, Value) ? Resp.SimpleEncode("OK") : "null";
    }

    private async Task ExpireSetCommand()
    {
        if (CommandOption == CommandOption.Px)
        {
            await Task.Delay(int.Parse(_value[10]));
            _store!.Remove(Key);
        }
    }

    private string HandleGetCommand()
    {
        var value = _store!.GetValue(Key);
        return value.Length > 0 ? Resp.BulkEncode(value) : Resp.NullEncode();
    }
    
    private string Key => _value[4];
    private string Value => _value[6];
    
    

    



}