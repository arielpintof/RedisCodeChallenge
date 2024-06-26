﻿using System.Net;
using System.Net.Sockets;
using System.Text;

namespace codecrafters_redis;

public static class ServerSettings
{
    public static int Port { get; set; } = 6379;
    public static string Role { get; set; } = "master";

    public static string MasterId { get;} = "8371b4fb1155b71f4a04d3e1bc3e18c4a990aeeb";
    public static string MasterHost { get; set; }
    public static int MasterPort { get; set; }

    public static List<string> Replicas { get; set; } = new();

    public const string EmptyRdb =
        "UkVESVMwMDEx+glyZWRpcy12ZXIFNy4yLjD6CnJlZGlzLWJpdHPAQPoFY3RpbWXCbQi8ZfoIdXNlZC1tZW3CsMQQAPoIYW9mLWJhc2XAAP/wbjv+wP9aog==";
    
    public static Task Configure(string[] args)
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
            MasterPort = int.Parse(args[roleIndex + 2]);
        }
        

        return Task.CompletedTask;
    }

    public static bool IsMaster() => Role.Equals("master");

    public static void AddReplicaPort(string port) => Replicas.Add(port);
    







}

