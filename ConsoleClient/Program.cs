using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ConsoleClient
{
    class Program
    {
        private const string APP_PATH = "http://localhost:5000";
        private static string token;

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Enter your username:");
                string userName = Console.ReadLine();

                Console.WriteLine("Enter your password:");
                string password = Console.ReadLine();

                string authStatus = performLogin(userName, password);

                if (authStatus == "Unauthorized")
                {
                    Console.WriteLine("Sorry, try again");
                    Console.ReadKey();
                    Console.Clear();
                    continue;
                }
                else
                {
                    Console.WriteLine("You are authenticated");
                    PrintCommands();
                }

                bool continueCommands = true;
                while (continueCommands)
                {
                    string command = Console.ReadLine();
                    string[] commandQuery = command.Split(' ');

                    switch (commandQuery[0])
                    {
                        case "-ac":
                            Console.WriteLine(CreateCollection());
                            break;
                        case "-af":
                            AddFeed(commandQuery[1], commandQuery[2]);
                            break;
                        case "-ga":
                            GetAllPosts();
                            break;
                        case "-gac":
                            GetAllPosts(commandQuery[1]);
                            break;
                        case "-q":
                            Environment.Exit(0);
                            break;
                        case "-help":
                            PrintCommands();
                            break;
                        default:
                            Console.WriteLine("There's problem with your operand, try again");
                            break;
                    }
                }
            }
        }

        private static void PrintCommands()
        {
            Console.WriteLine("Write one of the following commands:");
            Console.WriteLine("\t -ac to add new collection");
            Console.WriteLine("\t -af {id} {source} to add feed from {source} to collection with {id}");
            Console.WriteLine("\t -ga to get posts from all collections");
            Console.WriteLine("\t -gac {id} to get posts from collection with {id}");
            Console.WriteLine("\t -help to list all commands");
            Console.WriteLine("\t -q to exit");
            Console.WriteLine(" * example: -af 1 https://habr.com/rss/hubs/all/");
            Console.WriteLine(" * 0 id refers to general collections or \"All feeds\"");
            Console.WriteLine(" * please don't ommit http:// and https:// in links");

        }

        private static void GetAllPosts()
        {
            using (var client = CreateClient(token))
            {
                var response = client.GetAsync(APP_PATH + "/api/collections").Result;
                Console.WriteLine(FormatJson(response.Content.ReadAsStringAsync().Result));
            }
        }

        private static void GetAllPosts(string id)
        {
            using (var client = CreateClient(token))
            {
                var response = client.GetAsync(APP_PATH + "/api/collections/" + id).Result;
                Console.WriteLine(FormatJson(response.Content.ReadAsStringAsync().Result));
            }
        }

        private static void AddFeed(string id, string source)
        {
            using (var client = CreateClient(token))
            {
                var response = client.PostAsync(APP_PATH + "/api/collections/" + id + "/?feed=" + source,
                    new StringContent("")).Result;
            }
        }

        private static string CreateCollection()
        {
            using (var client = CreateClient(token))
            {
                var response = client.PostAsync(APP_PATH + "/api/collections",
                    new StringContent("new name", Encoding.UTF8, "application/json")).Result;

                return response.Content.ReadAsStringAsync().Result;
            }
        }

        private static string FormatJson(string json)
        {
            dynamic parsedJson = JsonConvert.DeserializeObject(json);
            return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
        }

        public static string performLogin(string username, string password)
        {
            string credentials = "{ 'username':'" + username + "', 'password':'" + password + "'}";
            using (var client = CreateClient(token))
            {
                var response = client.PostAsync(APP_PATH + "/api/auth",
                    new StringContent(credentials, Encoding.UTF8, "application/json")).Result;
                if (response.StatusCode.ToString() == "Unauthorized")
                    return response.StatusCode.ToString();
                dynamic data = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                token = data.token;
                return response.StatusCode.ToString();
            }
        }

        static HttpClient CreateClient(string accessToken = "")
        {
            var client = new HttpClient();
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            }

            return client;
        }
    }
}