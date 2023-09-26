using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using TerminalGPT.Options;
using TerminalGPT.Services;
using Spectre.Console;
using TerminalGPT.Extensions;

class Program
{
    static async Task Main(string[] args)
    {
        var host = new HostBuilder()
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.Configure<TerminalGptOptions>(hostContext.Configuration.GetSection("Options"));
                services.AddSingleton<IUserInputService, UserInputService>();
                services.AddSingleton<IChatCommandService, ChatCommandService>();
                services.AddSingleton<IExitService, ExitService>();
                services.AddSingleton<IOpenAIClientFactory, OpenAIClientFactory>();
                services.AddSingleton<IOpenAIService, OpenAiService>();
                services.AddSingleton<IUserService, UserService>();
                services.AddSingleton<IChatService, ChatService>();
                services.AddSingleton<IMenuService, MenuService>();
                services.AddSingleton<ITerminalChatService, TerminalChatService>();
                services.AddSingleton<MainService>();
            })
            .Build();

        using (var serviceScope = host.Services.CreateScope())
        {
            var services = serviceScope.ServiceProvider;

            try
            {
                var options = services.GetRequiredService<IOptions<TerminalGptOptions>>().Value;
                var userEnteredOptions = new TerminalGptOptions();
                var openAiService = services.GetRequiredService<IOpenAIService>();
                var mainService = services.GetRequiredService<MainService>();

                while (string.IsNullOrEmpty(options.ApiKey) || string.IsNullOrEmpty(options.OrgId) ||
                       options.Model == null)
                {
                    var missingOptionsPanel = new Panel(
                            $"[{(!options.ApiKey.IsNullOrWhiteSpace() ? "lime" : "orange3")}]{{{(!options.ApiKey.IsNullOrWhiteSpace() ? "x" : " ")}}}[/] API KEY: {(!options.ApiKey.IsNullOrWhiteSpace() ? options.ApiKey : "[red][bold]![/][/]")}\n" +
                            $"[{(!options.OrgId.IsNullOrWhiteSpace() ? "lime" : "orange3")}]{{{(!options.OrgId.IsNullOrWhiteSpace() ? "x" : " ")}}}[/] ORG ID: {(!options.OrgId.IsNullOrWhiteSpace() ? options.OrgId : "[red][bold]![/][/]")}\n" +
                            $"[{(options.Model is not null ? "lime" : "orange3")}]{{{(options.Model is not null ? "x" : " ")}}}[/] MODEL: {(options.Model is not null ? options.Model : "[red][bold]![/][/]")}\n" +
                            $"[{(!options.SystemPrompt.IsNullOrWhiteSpace() ? "lime" : "orange3")}]{{{(!options.SystemPrompt.IsNullOrWhiteSpace() ? "x" : " ")}}}[/] SYSTEM-PROMPT: {(!options.SystemPrompt.IsNullOrWhiteSpace() ? options.SystemPrompt : "[red][bold]![/][/]")}\n")
                        .Header("[orange3]ATTENTION:[/] [yellow]Please set the following options:[/]")
                        .Border(BoxBorder.Heavy)
                        .BorderColor(Color.Salmon1);


                    AnsiConsole.Render(missingOptionsPanel);
                    AnsiConsole.Render(new Rule());

                    if (AnsiConsole.Confirm("Would you like to set the options now?"))
                    {
                        while (string.IsNullOrEmpty(userEnteredOptions.ApiKey) ||
                               string.IsNullOrEmpty(userEnteredOptions.OrgId) ||
                               userEnteredOptions.Model == null)
                        {
                            userEnteredOptions = userEnteredOptions.Merge(options);
                            AnsiConsole.Clear();
                            missingOptionsPanel = new Panel(
                                    $"[{(!options.ApiKey.IsNullOrWhiteSpace() ? "lime" : "orange3")}]{{{(!options.ApiKey.IsNullOrWhiteSpace() ? "x" : " ")}}}[/] API KEY: {(!options.ApiKey.IsNullOrWhiteSpace() ? options.ApiKey : "[red][bold]![/][/]")}\n" +
                                    $"[{(!options.OrgId.IsNullOrWhiteSpace() ? "lime" : "orange3")}]{{{(!options.OrgId.IsNullOrWhiteSpace() ? "x" : " ")}}}[/] ORG ID: {(!options.OrgId.IsNullOrWhiteSpace() ? options.OrgId : "[red][bold]![/][/]")}\n" +
                                    $"[{(options.Model is not null ? "lime" : "orange3")}]{{{(options.Model is not null ? "x" : " ")}}}[/] MODEL: {(options.Model is not null ? options.Model : "[red][bold]![/][/]")}\n" +
                                    $"[{(!options.SystemPrompt.IsNullOrWhiteSpace() ? "lime" : "orange3")}]{{{(!options.SystemPrompt.IsNullOrWhiteSpace() ? "x" : " ")}}}[/] SYSTEM-PROMPT: {(!options.SystemPrompt.IsNullOrWhiteSpace() ? options.SystemPrompt : "[red][bold]![/][/]")}\n")
                                .Header("[orange3]ATTENTION:[/] [yellow]Please set the following options:[/]")
                                .Border(BoxBorder.Heavy)
                                .BorderColor(Color.Salmon1);


                            AnsiConsole.Render(missingOptionsPanel);
                            AnsiConsole.Render(new Rule());

                            var optionToSet = AnsiConsole.Prompt(
                                new SelectionPrompt<string>()
                                    .Title("Select an option to set")
                                    .PageSize(10)
                                    .AddChoices(new[]
                                    {
                                        "API Key", "Org ID", "Model", "System Prompt", "Exit"
                                    }));

                            AnsiConsole.MarkupLine($"[yellow]You selected {optionToSet}[/]");
                            var userInput = String.Empty;
                            switch (optionToSet)
                            {
                                case "API Key":
                                    userInput = AnsiConsole.Ask<string>(
                                        "Enter the new value for the option");
                                    userEnteredOptions.ApiKey = userInput;
                                    break;
                                case "Org ID":
                                    userInput = AnsiConsole.Ask<string>(
                                        "Enter the new value for the option");
                                    userEnteredOptions.OrgId = userInput;
                                    break;
                                case "Model":
                                    userInput = AnsiConsole.Prompt(new SelectionPrompt<string>()
                                        .Title("Select a model")
                                        .PageSize(10)
                                        .AddChoices(Enum.GetNames(typeof(GPTModel))));
                                    userEnteredOptions.Model = Enum.Parse<GPTModel>(userInput);
                                    break;
                                case "System Prompt":
                                    userInput = AnsiConsole.Ask<string>(
                                        "Enter the new value for the option");
                                    userEnteredOptions.SystemPrompt = userInput;
                                    break;
                                case "Exit":
                                    AnsiConsole.MarkupLine("[yellow]Goodbye...[/] [magenta1]:([/]");
                                    Task.Delay(1234).Wait();
                                    Environment.Exit(0);
                                    break;
                            }

                            options.Merge(userEnteredOptions);
                            if (options.Validate())
                                break;
                        }

                        AnsiConsole.MarkupLine("[lime]Options all set![/]");
                        AnsiConsole.MarkupLine("[yellow]Starting up Terminal.GPT...[/]");
                        var json = JsonConvert.SerializeObject(options, Formatting.Indented);

                        try
                        {
                            await File.WriteAllTextAsync("appsettings.json", json);
                        }
                        catch (Exception ex)
                        {
                            AnsiConsole.MarkupLine($"[red]An error occurred trying to save your settings[/]");
                        }

                        AnsiConsole.Clear();
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[yellow]Goodbye...[/] [magenta1]:([/]");
                        Task.Delay(1234).Wait();
                        Environment.Exit(0);
                    }
                }

                await AnsiConsole.Status().Spinner(Spinner.Known.Star)
                    .StartAsync("[lime][bold]TerminalGPT booting up...[/][/]", async spinnerCtx =>
                    {
                        await openAiService.ValidateApiKeyAsync();
                        Task.Delay(1000).Wait();
                        spinnerCtx.Spinner = Spinner.Known.Clock;
                        spinnerCtx.SpinnerStyle = Style.Parse("green");
                        spinnerCtx.Refresh();
                    });

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