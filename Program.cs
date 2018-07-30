using System.Collections.Generic;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace WahsKeyClubSite
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var list = new List<string>();
            list.AddRange(args);
            list.Add(" --server.urls");
            BuildWebHost(list.ToArray()).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}