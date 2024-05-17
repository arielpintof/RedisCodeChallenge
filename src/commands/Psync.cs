using System.Text;

namespace codecrafters_redis.commands;

public class Psync : ICommand
{
    
    public IEnumerable<byte[]> Handler()
    {
        var emptyRdb = ServerSettings.EmptyRdb.ToBinary();

        var response = new List<byte[]>
        {
            Resp.SimpleEncode($"FULLRESYNC {ServerSettings.MasterId} 0").AsByte(),
            $"${emptyRdb.Length}\r\n".AsByte().Concat(emptyRdb).ToArray()
        };

        return response;
    }
}