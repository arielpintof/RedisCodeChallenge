namespace codecrafters_redis;

public class RespValue
{
    public string? DataKey { get; set; }
    public string? DataValue { get; set; }
    public string? Argument { get; set; }
    public ExpirationType ExpirationType { get; set; }
    public int? ExpirationValue { get; set; }
    private IEnumerable<string> Value { get; set; }

    public RespValue(IEnumerable<string> value)
    {
        Value = value;
    }
    
    public Command Command => Value.ElementAt(2).ToLowerInvariant() switch
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
    
    private string? SetKey => Value.Count() >=4 ? Value.ElementAt(4) : string.Empty;
    
    private string? SetValue => Value.Count() >=6 ? Value.ElementAt(6) : string.Empty;
}