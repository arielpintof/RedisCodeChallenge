namespace codecrafters_redis;


//Clave valor
//Operaciones Set y Get
public class Store
{
    private readonly Dictionary<string, string> _storeValues = new();

    public bool Set(string key, string value) => _storeValues.TryAdd(key, value);

    public string GetValue(string key) => _storeValues.GetValueOrDefault(key, "(nil)");
    
}