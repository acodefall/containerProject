using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;

namespace ProduceToQAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebHostBuilder host = new WebHostBuilder();
            host.UseKestrel();
            host.UseContentRoot(Directory.GetCurrentDirectory());
            host.UseIISIntegration();
            host.UseStartup(typeof(Startup));

            IWebHost objWebHost = host.Build();

            objWebHost.Run();
        }
    }
}
