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
        private FileStream fs;

        public Server()
        {
            //loadConfig from local file
            ConfigManager configManager = new ConfigManager();
            _config = configManager.LoadConfig();
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
                    bool PlaceHolder = false;

                    int BytesToR = Convert.ToInt32(sr.ReadToEnd().ToString());
                    string input = sr.ReadToEnd();
                    string reply = (input == "ping") ? "pong" : getToken();
                    PlaceHolder = !(input == "ping"); 

                    //parse for ping/pong set placeholder as a result

                    Console.WriteLine("Client connected. Starting to receive the file");

                    if (PlaceHolder)
                    {
                        Authentication auth = new Authentication(_config.APIKEY);
                        string requestUri = _config.APIURL;

                        var token = auth.GetAccessToken();
                        if (_config.DebugLog)
                        {
                            Console.WriteLine("Token: {0}\n", token);
                            Console.WriteLine("Request Uri: " + requestUri + Environment.NewLine);
                        }

                        //return length of token and token itself to client
                        Console.WriteLine("Disconnected {0}", soc.RemoteEndPoint);
                    }
                }

                catch (Exception e)
                {
                    Console.WriteLine(e.Message);

                }
            }
        }



        private string getToken()
        {
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

        private byte[] toByteArray(string input)
        {
            var bytearray = new byte[1024];
            int i = 0;
            foreach (char c in input)
            {
                bytearray[i] = (byte)c;
                i++;
            }
            return bytearray;
        }

        private byte[] toOtherByteArray(string input)
        {
            var bytearray = new byte[input.Length];
            int i = 0;
            foreach (char c in input)
            {
                bytearray[i] = (byte)c;
                i++;
            }
            return bytearray;
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

        private string formatFilename(string input)
        {
            var rString = "";
            foreach (char x in input)
            {
                var y = x.ToString();
                if (y == ":" || y == "/" && rString.Length != 3 || y == ".")
                {//do nothing lol 
                }
                else { rString += x; }
            }
            return rString;
        }
    }
}
