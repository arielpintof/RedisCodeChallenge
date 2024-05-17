namespace codecrafters_redis.commands;

public class Ping : ICommand
{
    public IEnumerable<byte[]> Handler()
    {
        return new List<byte[]>
            { Resp.SimpleEncode("PONG").AsByte() };
    }
}