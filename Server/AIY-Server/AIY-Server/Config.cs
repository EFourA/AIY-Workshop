using System;
namespace AIY_Server
{
    public class Config
    {
        //simple poly class to hold all config props
        public int Port { get; set;}
        public string IPAddress { get; set; }
        public int NumberOfClients { get; set; }
        public string APIKEY { get; set; }
        public string APIURL{get; set; }
        public bool DebugLog { get; set; }
        
        public void SetProp(ConfigManager.ConfigOptions options, string value)
        {
            switch (options)
            {
                case (ConfigManager.ConfigOptions.apiKey):
                    this.APIKEY = value;
                    break;
                case (ConfigManager.ConfigOptions.apiUrl):
                    this.APIURL = value;
                    break;
                case (ConfigManager.ConfigOptions.ipAddress):
                    this.IPAddress = value;
                    break;
                case (ConfigManager.ConfigOptions.numberOfClients):
                    this.NumberOfClients = Convert.ToInt32(value);
                    break;
                case (ConfigManager.ConfigOptions.port):
                    this.Port = Convert.ToInt32(value);
                    break;
                case (ConfigManager.ConfigOptions.debugLog):
                    this.DebugLog = (value.ToUpper() == "T") ? true : false;
                    break;
            }
        }
    }
}
