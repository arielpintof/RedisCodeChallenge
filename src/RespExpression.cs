﻿using System.Text;

namespace codecrafters_redis;

public class RespExpression
{
    public readonly List<string> Value;

    public RespExpression(IEnumerable<string> value)
    {
        Value = value.ToList();
    }

    private Command Command => Value[2].ToLowerInvariant() switch
    {
        "Ping" => Command.Ping,
        "echo" => Command.Echo,
        "set" => Command.Set,
        "get" => Command.Get,
        "info" => Command.Info,
        "replconf" => Command.Replconf,
        "psync" => Command.Psync,
        _ => throw new ArgumentOutOfRangeException()
    };

    public IEnumerable<byte[]> GetMessage(Store store) =>
        Command switch
        {
            Command.Ping => HandlePingCommand(),
            Command.Echo => new List<byte[]>{Resp.BulkEncode(Value[4]).AsByte()},
            Command.Set => HandleSetCommand(store),
            Command.Get => HandleGetCommand(store),
            Command.Info => HandleInfoCommand(),
            Command.Replconf => HandleReplCommand(),
            Command.Psync => HandlePsyncCommand(),
            _ => throw new ArgumentOutOfRangeException()
        };

    private IEnumerable<byte[]> HandlePsyncCommand()
    {
        var emptyRdb = ServerSettings.EmptyRdb.ToBinary();
        Console.WriteLine($"Lenght: {Encoding.UTF8.GetString(emptyRdb).Length} ");

        var response = new List<byte[]>
        {
            Resp.SimpleEncode($"FULLRESYNC {ServerSettings.MasterId} 0").AsByte(),
            $"${Encoding.UTF8.GetString(emptyRdb).Length}{Resp.Separator}{Encoding.UTF8.GetString(emptyRdb)}".AsByte()
        };

        return response;
    }

    private IEnumerable<byte[]> HandleReplCommand()
    {
        return new List<byte[]>
        {
            Resp.SimpleEncode("OK").AsByte()
        };
    }

    private IEnumerable<byte[]> HandlePingCommand()
    {
        return new List<byte[]>
            { Resp.SimpleEncode("PONG").AsByte() };
    }

    private IEnumerable<byte[]> HandleInfoCommand()
    {
        if (!this.ValueHas("info") && !this.ValueHas("replication"))
            return new List<byte[]> { Resp.BulkEncode($"role:{ServerSettings.Role}").AsByte() };

        var response = new List<byte[]>
        {
            $"role:{ServerSettings.Role}".AsByte(),
            $"master_replid:{ServerSettings.MasterId}".AsByte(),
            "master_repl_offset:0".AsByte()
        };

        return new List<byte[]> { Resp.BulkEncode(string.Join(Resp.Separator, response)).AsByte() };
    }


    private IEnumerable<byte[]> HandleSetCommand(Store store)
    {
        var expiration = TryGetArgument("px", out var expirationValue)
            ? int.Parse(expirationValue)
            : 0;

        return new List<byte[]> { store.Set(GetKey, GetValue, expiration) ? Resp.SimpleEncode("OK").AsByte() : "null".AsByte() };
    }

    private IEnumerable<byte[]> HandleGetCommand(Store store)
    {
        var value = store.GetValue(GetKey);
        return new List<byte[]> { value!.Length > 0 ? Resp.BulkEncode(value).AsByte() : Resp.NullEncode().AsByte() };
    }

    private string GetKey => Value[4];
    private string GetValue => Value[6];

    private bool TryGetArgument(string key, out string value)
    {
        var index = Value.FindIndex(e => e == key);
        if (index is -1)
        {
            value = string.Empty;
            return false;
        }

        value = $"{index}";
        return true;
    }
}

public static class RespExtension
{
    public static bool ValueHas(this RespExpression expression, string argument)
    {
        return expression.Value.Any(e => e.Equals(argument));
    }
}