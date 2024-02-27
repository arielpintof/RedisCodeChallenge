namespace codecrafters_redis;


//Clave valor
//Operaciones Set y Get
public class Store
{
    private readonly Dictionary<string, Data> _storeValues = new();

    public bool Set(string key, string value, int expiration = 0) => 
        _storeValues.TryAdd(key, new Data(value, expiration));

    public string? GetValue(string key)
    {
        if (!_storeValues.TryGetValue(key, out var value)) return null;

        if (value.Expiration <= 0 || 
            value.CreatedAt.AddMilliseconds(value.Expiration) >= DateTimeOffset.Now)
        {
            return value.Value;
        }

        _storeValues.Remove(key, out _);

        return null;

    } 
    public bool Remove(string key) => _storeValues.Remove(key);

}