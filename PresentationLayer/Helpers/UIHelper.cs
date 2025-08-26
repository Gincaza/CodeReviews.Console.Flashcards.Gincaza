using Spectre.Console;

namespace PresentationLayer.Helpers;

public static class UIHelper
{
    public static void ShowError(string message)
    {
        AnsiConsole.MarkupLine($"[red]✗ {message}[/]");
    }

    public static void ShowSuccess(string message)
    {
        AnsiConsole.MarkupLine($"[green]✓ {message}[/]");
    }

    public static void ShowWarning(string message)
    {
        AnsiConsole.MarkupLine($"[yellow]{message}[/]");
    }

    public static void ShowInfo(string message)
    {
        AnsiConsole.MarkupLine($"[blue]{message}[/]");
    }

    public static void WaitForKeyPress()
    {
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[grey]Press any key to continue...[/]");
        Console.ReadKey();
    }

    public static void ClearScreen()
    {
        Console.Clear();
    }

    {
        Console.Clear();
        AnsiConsole.MarkupLine($"[bold blue]{title}[/]");
        AnsiConsole.WriteLine();
    }

    public static bool ConfirmAction(string message)
    {
        return AnsiConsole.Confirm(message);
    }

    public static string AskForInput(string prompt)
    {
        return AnsiConsole.Ask<string>(prompt);
    }
}