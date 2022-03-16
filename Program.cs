using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using System.Threading;
using Websocket.Client;
using Newtonsoft.Json;
using System.Text;
using System.Net;
using System.Collections.Generic;
using System.Diagnostics;

//@Ac3lives
//To do: Add an option to open specific sites

namespace HomemadeChocolateChip
{
    public class ChromeDebug
    {
        public string description {get; set;}
        public string devtoolsFrontendUrl {get; set;} 
        public string faviconUrl {get; set;} 
        public string id {get; set;}
        public string parentId {get; set;}
        public string title {get; set;}
        public string type {get; set;}
        public string url {get; set;}
        public string webSocketDebuggerUrl {get; set;}
    }
    //

    class Program
    {
        public static void Main(string[] args)
        { 
            if (args.Length != 3)
            {
                
                Console.WriteLine("Usage: \n HomemadeChocolateChip.exe <debugport> <killchrome>");
                Console.WriteLine("\t debugurl: The port which chrome debugger will utilize (eg 9444)");
                Console.WriteLine("\t killchrome: Terminate current chrome processes and restart them with debug enabled. [yes/no]");
                System.Environment.Exit(0);
            }
            try 
            {
                string debugPort = args[1];
                string debugUrl = $"http://127.0.0.1:{debugPort}/json";
                string killAndSpawnChrome = args[2]; //yes or no
                if (killAndSpawnChrome.Equals("yes"))
                {
                    Console.WriteLine("Terminating and respawning chrome...");
                    KnSChrome(debugPort);
                }
                Console.WriteLine("**---**CookieTime!**--**");
                getCookie(getwsUrl(debugUrl));
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
            }

            //"http://127.0.0.1:9444/json";
            

        }
        public static void KnSChrome(string debugPort)
        {
            foreach (var process in Process.GetProcessesByName("chrome"))
            {
                process.Kill();
                process.WaitForExit();
                Console.WriteLine("Terminated Chrome.exe process PID: {0}", process.Id);
            }

            string runpm = $"--restore-last-session --remote-debugging-port={debugPort}";
            var chromProc = System.Diagnostics.Process.Start(@"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe", runpm);

        }
        public static string getwsUrl(string debugUrl)
        {
            string response ="";
            using (var client = new WebClient())
            {
                response = client.DownloadString(debugUrl);
                //Console.WriteLine(response);
            }
            List<ChromeDebug> DebugObjects = (List<ChromeDebug>) JsonConvert.DeserializeObject(response, typeof(List<ChromeDebug>));
            //Console.WriteLine(DebugObjects[0].webSocketDebuggerUrl);
            return DebugObjects[0].webSocketDebuggerUrl;
        }
        public static void getCookie(string wsUrl)
        {
            var url = new Uri(wsUrl);
            var exitEvent = new ManualResetEvent(false);
            string json = @"{""id"": 1, ""method"": ""Network.getAllCookies""}";
            //Console.WriteLine("Sending message");
            using (var client = new WebsocketClient(url))
            {
                client.ReconnectTimeout = TimeSpan.FromSeconds(3);
                client.MessageReceived.Subscribe(msg => 
                {
                    Console.WriteLine($"Cookies Received:\n\n {msg}");
                });
                client.Start();

                Task.Run(() => client.Send(json));

                exitEvent.WaitOne(TimeSpan.FromSeconds(3));
            }
        }
    }
}
