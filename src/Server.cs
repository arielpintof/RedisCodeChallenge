using System.Net;
using System.Net.Sockets;
using System.Text;


Console.WriteLine("Logs from your program will appear here!");

const string response = "+PONG\r\n";

var server = new TcpListener(IPAddress.Any, 6379);
server.Start();
var socket = server.AcceptSocket();

socket.SendAsync(Encoding.UTF8.GetBytes(response), SocketFlags.None);
