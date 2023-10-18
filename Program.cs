using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using TerminalGPT.Options;
using TerminalGPT.Services;
using Spectre.Console;
using TerminalGPT.Extensions;
using TerminalGPT.Constants;

namespace TerminalGPT;

class Program
{
    static async Task Main(string[] args)
    {
        try
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile(AppConstants.SettingsPath, optional: false, reloadOnChange: true)
                .Build()
                ;

            var services = new ServiceCollection();
            services.Configure<TerminalGptOptions>(config.GetSection("Options"));
            services.AddSingleton<IUserInputService, UserInputService>();
            services.AddSingleton<IChatCommandService, ChatCommandService>();
            services.AddSingleton<IExitService, ExitService>();
            services.AddSingleton<IOpenAIClientFactory, OpenAIClientFactory>();
            services.AddSingleton<IOpenAIService, OpenAiService>();
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IChatService, ChatService>();
            services.AddSingleton<IMenuService, MenuService>();
            services.AddSingleton<ITerminalChatService, TerminalChatService>();
            services.AddSingleton<ISettingsService, SettingsService>();
            services.AddSingleton<MainService>();

            ServiceProvider provider = services.BuildServiceProvider();
            var optionsMonitor = provider.GetRequiredService<IOptions<TerminalGptOptions>>();
            var options = provider.GetRequiredService<IOptions<TerminalGptOptions>>().Value;
            var settingsService = provider.GetRequiredService<ISettingsService>();
            settingsService.SetSettings(options);
            var userEnteredOptions = new TerminalGptOptions();
            userEnteredOptions.Merge(options);
            var openAiService = provider.GetRequiredService<IOpenAIService>();
            var mainService = provider.GetRequiredService<MainService>();

            while (userEnteredOptions.Validate() == false)
            {
                var missingOptionsPanel = new Panel(
                        $"[{(!userEnteredOptions.ApiKey.IsNullOrWhiteSpace() ? "lime" : "orange3")}]{{{(!userEnteredOptions.ApiKey.IsNullOrWhiteSpace() ? "x" : " ")}}}[/] API KEY: {(!userEnteredOptions.ApiKey.IsNullOrWhiteSpace() ? userEnteredOptions.ApiKey : "[red][bold]![/][/]")}\n" +
                        $"[{(!userEnteredOptions.OrgId.IsNullOrWhiteSpace() ? "lime" : "orange3")}]{{{(!userEnteredOptions.OrgId.IsNullOrWhiteSpace() ? "x" : " ")}}}[/] ORG ID: {(!userEnteredOptions.OrgId.IsNullOrWhiteSpace() ? userEnteredOptions.OrgId : "[red][bold]![/][/]")}\n" +
                        $"[{(userEnteredOptions.Model is not null ? "lime" : "orange3")}]{{{(userEnteredOptions.Model is not null ? "x" : " ")}}}[/] MODEL: {(userEnteredOptions.Model is not null ? userEnteredOptions.Model : "[red][bold]![/][/]")}\n" +
                        $"[{(!userEnteredOptions.SystemPrompt.IsNullOrWhiteSpace() ? "lime" : "orange3")}]{{{(!userEnteredOptions.SystemPrompt.IsNullOrWhiteSpace() ? "x" : " ")}}}[/] SYSTEM-PROMPT: {(!userEnteredOptions.SystemPrompt.IsNullOrWhiteSpace() ? userEnteredOptions.SystemPrompt : "[red][bold]![/][/]")}\n")
                    .Header("[orange3]ATTENTION:[/] [yellow]Please set the following userEnteredOptions:[/]")
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
                                $"[{(!userEnteredOptions.ApiKey.IsNullOrWhiteSpace() ? "lime" : "orange3")}]{{{(!userEnteredOptions.ApiKey.IsNullOrWhiteSpace() ? "x" : " ")}}}[/] API KEY: {(!userEnteredOptions.ApiKey.IsNullOrWhiteSpace() ? userEnteredOptions.ApiKey : "[red][bold]![/][/]")}\n" +
                                $"[{(!userEnteredOptions.OrgId.IsNullOrWhiteSpace() ? "lime" : "orange3")}]{{{(!userEnteredOptions.OrgId.IsNullOrWhiteSpace() ? "x" : " ")}}}[/] ORG ID: {(!userEnteredOptions.OrgId.IsNullOrWhiteSpace() ? userEnteredOptions.OrgId : "[red][bold]![/][/]")}\n" +
                                $"[{(userEnteredOptions.Model is not null ? "lime" : "orange3")}]{{{(userEnteredOptions.Model is not null ? "x" : " ")}}}[/] MODEL: {(userEnteredOptions.Model is not null ? userEnteredOptions.Model : "[red][bold]![/][/]")}\n" +
                                $"[{(!userEnteredOptions.SystemPrompt.IsNullOrWhiteSpace() ? "lime" : "orange3")}]{{{(!userEnteredOptions.SystemPrompt.IsNullOrWhiteSpace() ? "x" : " ")}}}[/] SYSTEM-PROMPT: {(!userEnteredOptions.SystemPrompt.IsNullOrWhiteSpace() ? userEnteredOptions.SystemPrompt : "[red][bold]![/][/]")}\n")
                            .Header("[orange3]ATTENTION:[/] [yellow]Please set the following userEnteredOptions:[/]")
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
                                userEnteredOptions.ApiKey = userInput.StartsWith("sk")
                                    ? userInput
                                    : null;
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
                                ExitApplication();
                                break;
                        }
                    }

                    AnsiConsole.MarkupLine("[lime]Options all set![/]");
                    AnsiConsole.MarkupLine("[yellow]Starting up Terminal.GPT...[/]");
                    var json = JsonConvert.SerializeObject(userEnteredOptions, Formatting.Indented);

                    try
                    {

                        // Write the path to console
                        AnsiConsole.MarkupLine($"[yellow]Writing settings to {AppConstants.SettingsPath}[/]");
                        Task.Delay(2000).Wait();

                        File.WriteAllText(AppConstants.SettingsPath, json);
                    }
                    catch (Exception ex)
                    {
                        AnsiConsole.MarkupLine($"[red]An error occurred trying to save your settings:[/] {ex.Message}");
                        Task.Delay(2000).Wait();
                        ExitApplication();
                    }

                    AnsiConsole.Clear();
                }
                else
                {
                    AnsiConsole.MarkupLine("[yellow]Goodbye...[/] [magenta1]:([/]");
                    Task.Delay(1234).Wait();
                    ExitApplication();
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
                
                await mainService.Run();
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]An error occurred: {ex.Message}[/]");
            ExitApplication();
        }
    }

    static void ExitApplication()
    {
        System.Diagnostics.Process.GetCurrentProcess().Kill();
    }
}