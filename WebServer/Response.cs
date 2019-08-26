using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Diagnostics;

namespace WebServer
{
    class Response
    {
        private Byte[] data = null;
        private string status="";
        private string mime = "";
        private Response(string status,string mime,Byte[] data)
        {
            this.status = status;
            this.mime = mime;
            this.data = data;
        }
        public static Response From(Request request)
        {
            if (request == null)
                return MakeNullRequest();

            if (request.Type == "GET")
            {
                string file = Environment.CurrentDirectory + HTTPServer.WEB_DIR + request.URL;
                FileInfo f = new FileInfo(file);

                if (file.Contains("rest"))
                {
                    return RestApiGET(request.URL);
                }
                if (f.Exists && f.Extension.Contains("."))
                {
                    return MakeFromFile(f);
                }
                else
                {
                    DirectoryInfo di = new DirectoryInfo(f + "/");
                    if (!di.Exists)
                        return MakePageNotFound();
                    FileInfo[] files = di.GetFiles();
                    foreach(FileInfo ff in files)
                    {
                        string n = ff.Name;
                        if (n.Contains("index.html") || n.Contains("index.htm"))
                            return MakeFromFile(ff);
                    }
                }
                
            }
            if(request.Type == "POST")
            {
                string file = Environment.CurrentDirectory + HTTPServer.WEB_DIR + request.URL;
                if (file.Contains("rest"))
                {
                    return RestApiPOST(request.URL);
                }
            }
            else
                return MakeMethodNotAllowed();

            return MakePageNotFound();
        }
        //POST: URL/rest/
        private static Response RestApiPOST(string url)
        {
            string[] query = url.Split('/');
            if(query[2]=="Hi")
            {
                string temp = "POST Hello";
                byte[] d = Encoding.ASCII.GetBytes(temp);
                return new Response("200 OK", "text/html", d);
            }
            else
            {
                string temp = "POST Hi";
                byte[] d = Encoding.ASCII.GetBytes(temp);
                return new Response("200 OK", "text/html", d);
            }
        }
        //GET: URl/rest/emp1
        private static Response RestApiGET(string url)
        {
            var EmployeeDetails = new Dictionary<string, string>();
            EmployeeDetails.Add("emp1", "{EID : 373 , Name : XYZ , Batch : 1 , Type : Trainee}");
            EmployeeDetails.Add("emp2", "{EID : 2349 , Name : ABC ,Batch:2 ,Type : Intern}");

            string[] query = url.Split('/');
            //Debug.WriteLine("queryy "+query[2]);
            if(EmployeeDetails.ContainsKey(query[2]))
            {
                string temp = EmployeeDetails[query[2]];
                byte[] d= Encoding.ASCII.GetBytes(temp);
                return new Response("200 OK", "text/html", d);
            }
            else
            {
                string file = Environment.CurrentDirectory + HTTPServer.MSG_DIR + "405.html";
                FileInfo fi = new FileInfo(file);
                FileStream fs = fi.OpenRead();
                BinaryReader reader = new BinaryReader(fs);
                byte[] d = new byte[fs.Length];
                reader.Read(d, 0, d.Length);
                fs.Close();
                return new Response("405 Method Not allowed", "text/html", d);
            }
            
        }
        private static Response MakeFromFile(FileInfo f)
        {
            
            FileStream fs = f.OpenRead();
            BinaryReader reader = new BinaryReader(fs);
            byte[] d = new byte[fs.Length];
            reader.Read(d, 0, d.Length);
            fs.Close();
            return new Response("200 OK", "text/html", d);
        }

        private static Response MakeNullRequest()
        {
            string file = Environment.CurrentDirectory + HTTPServer.MSG_DIR + "400.html";
            FileInfo fi = new FileInfo(file);
            FileStream fs = fi.OpenRead();
            BinaryReader reader = new BinaryReader(fs);
            byte[] d = new byte[fs.Length];
            reader.Read(d, 0, d.Length);
            fs.Close();
            return new Response("400 Bad Request", "text/html", d);
        }

        private static Response MakePageNotFound()
        {
            string file = Environment.CurrentDirectory + HTTPServer.MSG_DIR + "404.html";
            FileInfo fi = new FileInfo(file);
            FileStream fs = fi.OpenRead();
            BinaryReader reader = new BinaryReader(fs);
            byte[] d = new byte[fs.Length];
            reader.Read(d, 0, d.Length);
            fs.Close();
            return new Response("404 Bad Page Not found", "text/html", d);
        }
        private static Response MakeMethodNotAllowed()
        {
            string file = Environment.CurrentDirectory + HTTPServer.MSG_DIR + "405.html";
            FileInfo fi = new FileInfo(file);
            FileStream fs = fi.OpenRead();
            BinaryReader reader = new BinaryReader(fs);
            byte[] d = new byte[fs.Length];
            reader.Read(d, 0, d.Length);
            fs.Close();
            return new Response("405 Method Not allowed", "text/html", d);
        }

        public void Post(NetworkStream stream)
        {
            StreamWriter writer = new StreamWriter(stream);
            writer.Flush();
            writer.WriteLine(String.Format("{0} {1} \r\nServer: {2}r\nContent-Type: {3}\rAccept-Ranges: bytes\r\nContent-Length:{4}\r\n",HTTPServer.VERSION,status,HTTPServer.NAME,mime,data.Length));
            //writer.Flush();
            stream.Write(data, 0, data.Length);
        }
    }
}
