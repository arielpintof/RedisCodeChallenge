namespace codecrafters_redis;

public class Data
{
    public string Value { get; set; }
    public int Expiration { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    
    public Data(string value, int expiration) {
        Value = value;
        Expiration = expiration;
        CreatedAt = DateTimeOffset.Now;
    }

}