using System;
using System.IO;
using System.Collections.Generic;

namespace AIY_Server
{
    public class ConfigManager
    {

        private readonly string _pathOfConfig = "Config.txt";

        public Config LoadConfig()
        {
            Config config = new Config();
            StreamReader sr = new StreamReader(_pathOfConfig);
            List<string> configText = new List<string>();
            while (!sr.EndOfStream)
            {
                configText.Add(sr.ReadLine());
            }

            foreach(string s in configText)
            {
                //if begins with #, then it's a comment
                if (s != "" && s[0].ToString() != "#")
                {
                    var ts = s[0].ToString();
                    if (int.TryParse(ts, out int answer))
                    {
                        if (Enum.IsDefined(typeof(ConfigOptions), answer))
                            config.SetProp((ConfigOptions)answer, s.Substring(2));
                    }
                }
            }
            sr.Close();
            return config;
        }

        public void OverWriteConfigFile(string FilePathOfReplacementConfig)
        {
            //pass in a file path and whatever is at that filepath will replace
            //what is currently in the config file
            try
            {
                StreamReader sr = new StreamReader(FilePathOfReplacementConfig);
                List<string> cfg = new List<string>();
                while (!sr.EndOfStream)
                {
                    cfg.Add(sr.ReadLine());
                }
                sr.Close();

                StreamWriter sw = new StreamWriter(_pathOfConfig);
                foreach (string x in cfg)
                {
                    sw.WriteLine(x);
                }
                sw.Close();
            } catch(Exception e)
            {
                Console.WriteLine("Error when overwriting config");
                Console.WriteLine(e.Message);
            }
        }

        public void OutputSavedConfig() 
        {
            Console.WriteLine("");
            try
            {
                StreamReader sr = new StreamReader(_pathOfConfig);
                while (!sr.EndOfStream)
                {
                    Console.WriteLine(sr.ReadLine());
                }
                sr.Close();
                Console.WriteLine("");
                Console.WriteLine("End of File, press enter to continue...");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("");
                Console.WriteLine("Exception: {0}",e.Message);
                Console.WriteLine("File was not found, check 'Config.txt' exists?");
            }
            Console.WriteLine("");
        }

        public void ConfigManipMenu()
        {
            //Get choice

            //allow overwrite for choice y/n prompt

            //call OverwriteConfigOption with choice & value 

            //ask if they want to make any more changes

            //exit
        }


        public void OverWriteConfigOption(ConfigOptions configToOverWrite, string value)
        {
            //From menu, allow person to choose a speicifc config to overwrite
            //todo
            throw new NotImplementedException();
        }

        public enum ConfigOptions
        {
            port = 1,
            ipAddress = 2,
            numberOfClients =3,
            apiKey = 4,
            apiUrl = 5,
            debugLog = 6
        }
    }
}
