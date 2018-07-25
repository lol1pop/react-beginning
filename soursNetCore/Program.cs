using System;
using System.IO;
using System.Text;
using fastJSON;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace DeskAlerts
{
    public class Program
    {
        public static UserManager user;
        public static DeviceManager device;
        public static DispatchManager dispatch;
        public static ContentManager content;


        public static void Main(string[] args)
        {
            Console.InputEncoding = Encoding.Unicode;
            Console.OutputEncoding = Encoding.Unicode;

            JSON.Parameters.UseExtensions = false;
            JSON.Parameters.UseEscapedUnicode = false;

            user = new UserManager();
            device = new DeviceManager(user);
            dispatch = new DispatchManager(device);
            content = new ContentManager(user, device, dispatch);

          //  BuildWebHost(args).Run();  // Встроеный небольшой логер  
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("hosting.json", true)
                .Build();
            var host = new WebHostBuilder()
            .UseKestrel()
            .UseConfiguration(config)         
            .UseContentRoot(Directory.GetCurrentDirectory())
            .UseIISIntegration()
            .UseApplicationInsights()
            .UseStartup<Startup>()
            .Build();

            host.Run();
        }


        public static IWebHost BuildWebHost(string[] args) {
            return WebHost.CreateDefaultBuilder(args)
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .UseIISIntegration()
                .UseApplicationInsights()
                .Build();
        }
    }
}