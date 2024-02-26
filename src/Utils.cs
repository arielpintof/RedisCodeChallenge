namespace codecrafters_redis;

public static class Utils
{
    public const string Separator = "\r\n";

    public static RespExpression RespDecode(string value)
    {
        return new RespExpression(value.Split(Separator));
    }

    public static string RespEncode(string value)
    {
        return $"${value.Length}{Separator}{value}{Separator}";
    }
}