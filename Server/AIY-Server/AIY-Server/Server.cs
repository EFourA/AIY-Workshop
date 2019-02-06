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

                    //var data = new byte[BytesToR];

                    var data = sr.ReadToEnd().


                    Console.WriteLine("Client connected. Starting to receive the file");

                



                    // Note: Sign up at https://azure.microsoft.com/en-us/try/cognitive-services/ to get a subscription key.  
                    if (PlaceHolder)
                    {
                        Authentication auth = new Authentication(_config.APIKEY);
                        string requestUri = _config.APIURL;
                        string contentType = @"audio/wav; codec=""audio/pcm""; samplerate=16000";
                        string responseString;

                        try
                        {
                            var token = auth.GetAccessToken();
                            if (_config.DebugLog)
                            {
                                Console.WriteLine("Token: {0}\n", token);
                                Console.WriteLine("Request Uri: " + requestUri + Environment.NewLine);
                            }

                            HttpWebRequest request = null;
                            request = (HttpWebRequest)HttpWebRequest.Create(requestUri);
                            request.SendChunked = true;
                            request.Accept = @"application/json;text/xml";
                            request.Method = "POST";
                            request.ProtocolVersion = HttpVersion.Version11;
                            request.ContentType = contentType;
                            request.Date = DateTime.Now;
                            request.Headers["Authorization"] = "Bearer " + token;
                            request.Date = DateTime.Now;

                            using (fs = new MemoryStream(data)
                            {
                                buffer = null;
                                int bytesRead = 0;
                                using (Stream requestStream = request.GetRequestStream())
                                {
                                    buffer = new Byte[checked((uint)Math.Min(1024, (int)fs.Length))];
                                    while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) != 0)
                                    {
                                        requestStream.Write(buffer, 0, bytesRead);
                                    }
                                    requestStream.Flush();
                                }

                                Console.WriteLine("Request sent from {0}", soc.RemoteEndPoint);

                                using (WebResponse response = request.GetResponse())
                                {
                                    using (StreamReader _sr = new StreamReader(response.GetResponseStream()))
                                    {
                                        responseString = _sr.ReadToEnd();
                                    }

                                    if (responseString != "")
                                    {
                                        if (_config.DebugLog)
                                        {
                                            Console.WriteLine("Response:");
                                            Console.WriteLine(((HttpWebResponse)response).StatusCode);
                                            Console.WriteLine(responseString);
                                        }

                                        if (responseString.Contains("DisplayText") && responseString.Contains("Success"))
                                        {
                                            string tempStr = "";
                                            int i = 46;
                                            while (tempStr == "" || tempStr[tempStr.Length - 1] != '"')
                                            {
                                                tempStr += responseString[i];
                                                i++;
                                            }
                                            tempStr.Remove(tempStr.Length - 1);
                                            responseString = tempStr;
                                            string bytesToSend = responseString.Length.ToString();
                                            while (bytesToSend.Length < 6)
                                            {
                                                bytesToSend = "0" + bytesToSend;
                                            }
                                            Console.WriteLine("{0} said: \"{1}", soc.RemoteEndPoint, responseString);
                                            sw.Write(bytesToSend);
                                            sw.Write(responseString);
                                            Console.ReadLine();
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                            Console.WriteLine(ex.Message);
                            Console.ReadLine();
                        }
                    }
                    Console.WriteLine("Disconnected {0}", soc.RemoteEndPoint);
                }
                catch (Exception e)
                {
                    //socket died somehow, respawn it to allow reconnection
                    Console.WriteLine("Disconnected {0} with exception {1}", soc.RemoteEndPoint, e.Message);
                }

            }



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
