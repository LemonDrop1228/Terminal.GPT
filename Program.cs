using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using TerminalGPT.Options;
using TerminalGPT.Services;
using Spectre.Console;

class Program
{
    static async Task Main(string[] args)
    {
        var host = new HostBuilder()
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false);
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.Configure<TerminalGptOptions>(hostContext.Configuration.GetSection("Options"));
                services.AddSingleton<IOpenAIClientFactory, OpenAIClientFactory>();
                services.AddSingleton<IOpenAIService, OpenAiService>();
                services.AddSingleton<IUserService, UserService>();
                services.AddSingleton<IChatService, ChatService>();
                services.AddSingleton<MainService>();
            })
            .Build();

        using (var serviceScope = host.Services.CreateScope())
        {
            var services = serviceScope.ServiceProvider;

            try
            {
                var options = services.GetRequiredService<IOptions<TerminalGptOptions>>().Value;
                var mainService = services.GetRequiredService<MainService>();
                if (string.IsNullOrEmpty(options.ApiKey) || string.IsNullOrEmpty(options.OrgId) || options.Model == null)
                {
                    AnsiConsole.MarkupLine("[red]Required settings are missing. Press any key to exit.[/]");
                    Console.ReadKey();
                    return;
                }
                else
                {
                    await mainService.Initialize();
                }
                var now = DateTime.Now;
                // listen for ctrl+c and exit
                Console.CancelKeyPress += (sender, eventArgs) =>
                {
                    eventArgs.Cancel = true;
                    Environment.Exit(0);
                };
                
                await mainService.Run();
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]An error occurred: {ex.Message}[/]");
            }
        }
    }
}
