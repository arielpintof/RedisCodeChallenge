using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace codecrafters_redis;

public class RespExpression
{
    private readonly ImmutableArray<string> _value;
    private readonly Store? _store;

    public RespExpression(IEnumerable<string> value)
    {
        _value = value.ToImmutableArray();
    }

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

    public string GetMessage() => Command switch
    {
        Command.Ping => Resp.SimpleEncode("PONG"),
        Command.Echo => Resp.BulkEncode(_value[4]),
        Command.Set => HandleSetCommand(),
        Command.Get => HandleGetCommand(),
        _ => throw new ArgumentOutOfRangeException()
    };

    private string HandleSetCommand() => 
        _store!.Set(_value[4], _value[6]) ? Resp.SimpleEncode("OK") : "null";
    
    private string HandleGetCommand() => Resp.BulkEncode(_store!.GetValue(_value[3]));
    
}