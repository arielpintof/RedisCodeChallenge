namespace codecrafters_redis.commands;

public class Set : ICommand
{
    private readonly Store _store;
    private readonly List<string> _input;
    private readonly string _value;
    private readonly string _key;

    public Set(Store store, List<string> input, string key, string value)
    {
        _store = store;
        _input = input;
        _key = key;
        _value = value;
    }
    public IEnumerable<byte[]> Handler()
    {
        var expiration = TryGetArgument("px", out var expirationValue)
            ? int.Parse(expirationValue)
            : 0;
        
        Propagation.AddCommand(string.Join("", _input));

        return new List<byte[]> { _store.Set(_key, _value, expiration) ? Resp.SimpleEncode("OK").AsByte() : "null".AsByte() };
    }
    
    private bool TryGetArgument(string key, out string value)
    {
        var index = _input.FindIndex(e => e == key);
        if (index is -1)
        {
            value = string.Empty;
            return false;
        }

        value = $"{index}";
        
        return true;
    }
}