namespace codecrafters_redis.commands;

public interface ICommand
{
    IEnumerable<byte[]> Handler();
}