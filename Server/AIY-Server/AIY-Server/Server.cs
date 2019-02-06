using System;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Collections.Generic;
using System.Net;

namespace AIY_Server
{
    public class Server
    {
        private readonly Config _config;
        private bool Run;
        private List<Thread> pool = new List<Thread>();
        static TcpListener listener;

        public Server()
        {
            _config = new ConfigManager().LoadConfig();
            pool = new List<Thread>();
        }

        public void Start()
        {
            Run = true;
            listener = new TcpListener(parseIP(), _config.Port);
            listener.Start();
            Console.WriteLine("Server started at {0} on port {1}", _config.IPAddress, _config.Port);
            Console.WriteLine("");

            for (int i = 0; i < _config.NumberOfClients; i++)
            {
                Thread T = new Thread(new ThreadStart(Service));
                pool.Add(T);
                T.Start();
                Console.WriteLine("Started Thread {0}", T.ManagedThreadId);
                Console.WriteLine("");
            }
        }

        private void Service()
        {
            while (Run)
            {
                Socket soc = listener.AcceptSocket();
                Console.WriteLine("Connected {0}!", soc.RemoteEndPoint);
                try
                {
                    Stream S = new NetworkStream(soc);
                    StreamReader sr = new StreamReader(S);
                    StreamWriter sw = new StreamWriter(S);
                    sw.AutoFlush = true;

                    Console.WriteLine("Connected {0}", soc.RemoteEndPoint);
                    string input = sr.ReadToEnd();
                    string reply = (input == "ping") ? "pong" : getToken();
                    sw.Write(makeLenStr(reply));
                    sw.Write(reply);
                    Console.WriteLine("Disconnected {0}", soc.RemoteEndPoint);
                }

                catch (Exception e)
                {
                    Console.WriteLine(e.Message);

                }
            }
        }

        //Makes a packet intended to let the client
        //know how many bytes to expect in the api key
        //about to be returned
        private string makeLenStr(string lenstr)
        {
            while (lenstr.Length < 6)
            {
                lenstr = "0" + lenstr;
            }
            return lenstr;
        }

        private string getToken()
        {
            //As much as I'd love to only have to make an auth once
            //And re-use it across all threads, it just ain't happening chief
            Authentication auth = new Authentication(_config.APIKEY);
            string requestUri = _config.APIURL;
            var token = auth.GetAccessToken();
            if (_config.DebugLog)
            {
                Console.WriteLine("Token: {0}\n", token);
                Console.WriteLine("Request Uri: " + requestUri + Environment.NewLine);
            }
            return token;
        }

        public void Stop()
        {
            Run = false;
            listener.Stop();
        }

        private System.Net.IPAddress parseIP()
        {
            if (System.Net.IPAddress.TryParse(_config.IPAddress, out System.Net.IPAddress address))
            {
                return address;
            }
            else
            {
                while (true)
                {
                    Drawing.DrawHeader();
                    Console.WriteLine("Error, IP from file was not parsed correctly, manually enter and try again now");
                    string attempt = Console.ReadLine();
                    if (System.Net.IPAddress.TryParse(attempt, out System.Net.IPAddress re_address))
                    {
                        _config.IPAddress = attempt;
                        return re_address;
                    }
                }
            }
        }

    }
}
