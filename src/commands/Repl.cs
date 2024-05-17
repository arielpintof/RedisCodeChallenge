namespace codecrafters_redis.commands;

public class Repl : ICommand
{
    private readonly string _key;
    private readonly string _value;
    
    public Repl(string key, string value)
    {
        _key = key;
        _value = value;
    }
    
    public IEnumerable<byte[]> Handler()
    {
        if (_key.ToLowerInvariant().Equals("listening-port"))
        {
            ServerSettings.AddReplicaPort(_value);
        }
        
        return new List<byte[]>
        {
            Resp.SimpleEncode("OK").AsByte()
        };
    }
}