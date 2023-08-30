using SocketServer.Models;
using System;
namespace SocketServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Server...");
            Server server = new Server();
            server.ServerStart();

            Console.ReadLine();
            Console.WriteLine("Closing Server...");
            server.Close();
        }
    }
}
