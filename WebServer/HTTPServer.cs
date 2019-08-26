using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace WebServer
{
    class HTTPServer
    {
        public const string MSG_DIR= "/root/msg/";
        public const string WEB_DIR = "/root/web/";
        public const string VERSION = "HTTP/1.1";
        public const string NAME = "Demo Http server v0.1";
        private bool running = false;
        private TcpListener listener;
        public HTTPServer(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
        }
        public void Start()
        {
            Thread serverThread = new Thread(new ThreadStart(Run));
            serverThread.Start();
        }
        private void Run()
        {
            running = true;
            listener.Start();

            while(running)
            {
                Console.WriteLine("Waiting for Connection");
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("Client Connected");
                HandleClient(client);
                client.Close();
            }
            running = false;
            listener.Stop();
        }

        private void HandleClient(TcpClient client)
        {
            StreamReader reader = new StreamReader(client.GetStream());
            string msg = "";
            while(reader.Peek()!=-1)
            {
                msg += reader.ReadLine() + "\n";
            }
            Debug.WriteLine("Request: \n"+ msg);

            Request req = Request.GetRequest(msg);
            Response resp = Response.From(req);
            resp.Post(client.GetStream());
        }
    }
}
