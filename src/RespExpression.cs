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
        _ => throw new ArgumentOutOfRangeException()
    };

    public string GetMessage() => Command switch
    {
        Command.Ping => "+PONG\r\n",
        Command.Echo => Utils.RespEncode(_value[4]),
        _ => throw new ArgumentOutOfRangeException()
    };
}