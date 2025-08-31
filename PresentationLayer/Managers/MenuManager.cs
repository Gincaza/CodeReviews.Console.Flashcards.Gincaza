using Spectre.Console;

namespace PresentationLayer.Managers;

public class MenuManager
{
    public string ShowMainMenu()
    {
        AnsiConsole.MarkupLine("[bold yellow]Welcome to Flashcards App![/]");
        AnsiConsole.WriteLine();

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("What would you like to do?")
                .AddChoices(new[]
                {
                    "Create deck",
                    "Manage Deck", 
                    "See Decks",
                    "Start Study Lesson",
                    "View Study History",
                    "Exit"
                }));

        return choice;
    }

    public string ShowManageDeckMenu()
    {
        var action = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("What would you like to do?")
                .AddChoices(new[]
                {
                    "Create Card",
                    "Edit Card",
                    "Delete Card",
                    "Delete Deck",
                    "Back to Main Menu"
                }));

        return action;
    }

    public void ShowWelcomeBanner()
    {
        AnsiConsole.Write(
            new FigletText("Flashcards")
                .Centered()
                .Color(Color.Blue));

        AnsiConsole.WriteLine();
    }

    public void ShowGoodbye()
    {
        AnsiConsole.MarkupLine("[green]Goodbye![/]");
    }
}