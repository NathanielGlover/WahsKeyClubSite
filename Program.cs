using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

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