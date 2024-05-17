namespace codecrafters_redis.commands;


public class Echo : ICommand, IDisposable
{
    private readonly string _key;

    public Echo(string key)
    {
        _key = key;
    }
    
    public IEnumerable<byte[]> Handler()
    {
        return new [] { Resp.BulkEncode(_key).AsByte() };
    }

    public virtual void Dispose()
    {
        // TODO release managed resources here
    }
}