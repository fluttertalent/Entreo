
using Entreo.Services.Services;

namespace WebApp.Entreo.Configuration
{
    public class AppSettings
    {
        public static IConfiguration Configuration { get; }
        public static EmailSettings EmailSettings { get; set; }

        static AppSettings()
        {
            Configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
                    .AddJsonFile($"appsettings.{Environment.MachineName}.json", optional: true)
                    .Build();

            EmailSettings = new EmailSettings();
            Configuration.GetSection(nameof(EmailSettings)).Bind(EmailSettings);
        }
    }

}