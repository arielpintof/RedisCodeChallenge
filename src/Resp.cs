namespace codecrafters_redis;

public static class Resp
{
    public const string Separator = "\r\n";

    public static RespExpression Decode(string value)
    {
        return new RespExpression(value.Split(Separator));
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
        return $"$-1{Separator}";
    }
    
    public static string ArrayEncode(List<string> value)
    {
        var elements = value.Select(element => $"${element.Length}{Separator}{element}{Separator}").ToList();
        var joinedElements = string.Join("", elements);
        
        Console.WriteLine($"*{value.Count}{Separator}{joinedElements}");
        return $"*{value.Count}{Separator}{joinedElements}";
    }
}