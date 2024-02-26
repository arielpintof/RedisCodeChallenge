namespace codecrafters_redis;

public static class Resp
{
    private const string Separator = "\r\n";

    public static RespExpression Decode(string value, Store store)
    {
        return new RespExpression(value.Split(Separator), store);
    }

    public static string BulkEncode(string value)
    {
        return $"${value.Length}{Separator}{value}{Separator}";
    }
    
    public static string SimpleEncode(string value)
    {
        return $"+{value}{Separator}";
    }

    public static string NullEncode()
    {
        return $"-1{Separator}";
    }
}