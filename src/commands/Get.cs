namespace codecrafters_redis.commands;

public class Get : ICommand
{
    private readonly Store _store;
    private readonly string _key;
    
    public Get(Store store, string key)
    {
        _store = store;
        _key = key;
    }
    
    public IEnumerable<byte[]> Handler()
    {
        var value = _store.GetValue(_key);
        
        return new [] { value!.Length > 0 ? Resp.BulkEncode(value).AsByte() : Resp.NullEncode().AsByte() };
    }
}