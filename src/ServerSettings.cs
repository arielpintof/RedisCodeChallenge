namespace codecrafters_redis;

public static class ServerSettings
{
    public static int Port { get; set; } = 6379;
    public static string Role { get; set; } = "master";
    public static string MasterHost { get; set; }
    public static string MasterPort { get; set; }
    

    public static void Configure(string[] args)
    {
        var portIndex = Array.IndexOf(args, "--port");
        if (portIndex != -1)
        {
            Port = int.Parse(args[portIndex + 1]);
        }
        
        var roleIndex = Array.IndexOf(args, "--replicaof");
        if (roleIndex != -1)
        {
            Role = "slave";
            MasterHost = args[roleIndex + 1];
            MasterPort = args[roleIndex + 2];
        }
    }

    public static bool IsMaster() => Role.Equals("master");




}

