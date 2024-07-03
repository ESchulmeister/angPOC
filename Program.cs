using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace angPOC
{
    public class Program
    {
        public static void Main(string[] args)                                                                                          
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
              .ConfigureLogging(logging =>
               {
                     logging.SetMinimumLevel(LogLevel.Error);
                   logging.AddLog4Net(new Log4NetProviderOptions("log4net.config"));
               })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.SuppressStatusMessages(true);//disable the status messages
                webBuilder.UseStartup<Startup>();
            });
    }
}
