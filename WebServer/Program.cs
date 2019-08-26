using System;
using System.Collections.Generic;
using System.Text;

namespace WebServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting server on port 8090");
            HTTPServer server = new HTTPServer(8090);
            server.Start();
        }
    }
}
