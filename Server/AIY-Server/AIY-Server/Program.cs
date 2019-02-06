using System;

namespace AIY_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            bool Quit = false;
            while (!Quit)
            {
                var Choice = GetOptions();
                Console.Write(Choice.ToString());
                Console.WriteLine("");

                switch (Choice)
                {
                    case Options.Run:
                        Run();
                        break;

                    case Options.DisplayConfig:
                        DisplayConfig();
                        break;

                    case Options.RunConfigMenu:
                        ConfigMenu();
                        break;

                    case Options.Quit:
                        Quit = true;
                        break;

                    default:
                        //shouldn't get here
                        break;
                }
            }
        }


        #region CoreCode

        public static void Run()
        {
            Server server = new Server();
            server.Start();

            Console.WriteLine("Send any key to stop the server");
            Console.WriteLine("");
            Console.ReadLine();
            Console.WriteLine("");
            Console.WriteLine("Server stopped: {0}", DateTime.Now.ToLongTimeString());
            server.Stop();
        }

        public static void DisplayConfig()
        {
            ConfigManager configManager = new ConfigManager();
            configManager.OutputSavedConfig();
        }

        public static void ConfigMenu()
        {
            //todo
            throw new NotImplementedException();
        }

        #endregion


        #region MenuRegion

        public enum Options
        {
            Run = 1,
            DisplayConfig = 2,
            RunConfigMenu = 3,
            Quit = -1
        }

        static Options GetOptions()
        {
            while (true)
            {
                DrawMenu();
                if (int.TryParse(Console.ReadLine(), out int answer))
                {
                    if (Enum.IsDefined(typeof(Options), answer))
                        return (Options)answer;
                }
            }
        }

        static void DrawMenu()
        {
            Drawing.DrawHeader();
            Console.WriteLine("AIY-Server 1.0 - James Matchett");
            Drawing.DrawHeader();
            Console.WriteLine("");
            Console.WriteLine("1. Run");
            Console.WriteLine("");
            Console.WriteLine("2. Display config");
            Console.WriteLine("");
            Console.WriteLine("3. Run Config menu");
            Console.WriteLine("");
            Console.WriteLine("-1. Quit");
            Drawing.DrawHeader();
            Console.WriteLine("");
        }

        #endregion
    }
}
