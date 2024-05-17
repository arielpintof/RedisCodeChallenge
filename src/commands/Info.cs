namespace codecrafters_redis.commands;

public class Info : ICommand
{
    private readonly RespExpression _respExpression;

    public Info(RespExpression respExpression)
    {
        _respExpression = respExpression;
    }
    
    public IEnumerable<byte[]> Handler()
    {
        if (!_respExpression.ValueHas("info") && !_respExpression.ValueHas("replication"))
            return new List<byte[]> { Resp.BulkEncode($"role:{ServerSettings.Role}").AsByte() };

        var response = new List<string>
        {
            $"role:{ServerSettings.Role}",
            $"master_replid:{ServerSettings.MasterId}",
            "master_repl_offset:0"
        };

        return new List<byte[]> { Resp.BulkEncode(string.Join(Resp.Separator, response)).AsByte() };
    }
}