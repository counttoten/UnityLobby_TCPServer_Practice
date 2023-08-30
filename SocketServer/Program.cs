using SocketServer.Models;
using System;
namespace SocketServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            
            Server server = new Server();
            server.ServerStart();

            Console.ReadLine();
            server.Close();
        }
    }
}
