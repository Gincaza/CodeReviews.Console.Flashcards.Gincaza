using Business_Logic;
using Business_Logic.DTO;
using Business_Logic.Messages;
using Spectre.Console;

namespace PresentationLayer;

public class FlashcardsUI
{
    private readonly BusinessLogic _businessLogic;

    public FlashcardsUI(BusinessLogic businessLogic)
    {
        _businessLogic = businessLogic;
    }

    public void Run()
    {
        AnsiConsole.Write(
            new FigletText("Flashcards")
                .Centered()
                .Color(Color.Blue));

        AnsiConsole.WriteLine();

        while (true)
        {
            var choice = ShowMainMenu();

            try
            {
                switch (choice)
                {
                    case "Create deck":
                        CreateDeck();
                        break;
                    case "Manage Deck":
                        ManageDeck();
                        break;
                    case "See Decks":
                        ShowDecks();
                        break;
                    case "Start Study Lesson":
                        StartStudyLesson();
                        break;
                    case "Exit":
                        AnsiConsole.MarkupLine("[green]Goodbye![/]");
                        return;
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            }

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[grey]Press any key to continue...[/]");
            Console.ReadKey();
            Console.Clear();
        }
    }

    private string ShowMainMenu()
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
                    "Exit"
                }));

        return choice;
    }

    private void CreateDeck()
    {
        Console.Clear();
        AnsiConsole.MarkupLine("[bold blue]Create New Deck[/]");
        AnsiConsole.WriteLine();

        var title = AnsiConsole.Ask<string>("Enter deck [green]title[/]:");

        AnsiConsole.Status()
            .Start("Creating deck...", ctx =>
            {
                var result = _businessLogic.CreateDeck(title);

                if (result.Success)
                {
                    AnsiConsole.MarkupLine($"[green]✓ {result.Message}[/]");
                }
                else
                {
                    AnsiConsole.MarkupLine($"[red]✗ {result.Message}[/]");
                }
            });
    }

    private void ManageDeck()
    {
        Console.Clear();
        AnsiConsole.MarkupLine("[bold blue]Manage Deck[/]");
        AnsiConsole.WriteLine();

        var decks = _businessLogic.ListAllDecks();

        if (!decks.Any() || decks.All(d => d == null))
        {
            AnsiConsole.MarkupLine("[yellow]No decks available. Please create a deck first.[/]");
            return;
        }

        var deckChoices = decks.Where(d => d != null).Select(d => $"{d!.Id} - {d.Title}").ToList();
        var selectedDeck = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select a deck to manage:")
                .AddChoices(deckChoices));

        var deckId = int.Parse(selectedDeck.Split(" - ")[0]);
        ManageDeckCards(deckId);
    }

    private void ManageDeckCards(int deckId)
    {
        while (true)
        {
            Console.Clear();
            AnsiConsole.MarkupLine($"[bold blue]Managing Deck (ID: {deckId})[/]");
            AnsiConsole.WriteLine();

            var cards = _businessLogic.ListAllFlashCards(deckId);

            // Show current cards
            if (cards.Any() && cards.Any(c => c != null))
            {
                var table = new Table();
                table.AddColumn("Word");
                table.AddColumn("Translation");

                foreach (var card in cards.Where(c => c != null))
                {
                    table.AddRow(card!.Word, card.Translation);
                }

                AnsiConsole.Write(table);
                AnsiConsole.WriteLine();
            }
            else
            {
                AnsiConsole.MarkupLine("[yellow]No cards in this deck yet.[/]");
                AnsiConsole.WriteLine();
            }

            var action = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("What would you like to do?")
                    .AddChoices(new[]
                    {
                        "Create Card",
                        "Edit Card",
                        "Delete Card",
                        "Back to Main Menu"
                    }));

            switch (action)
            {
                case "Create Card":
                    CreateCard(deckId);
                    break;
                case "Edit Card":
                    EditCard(deckId);
                    break;
                case "Delete Card":
                    DeleteCard(deckId);
                    break;
                case "Back to Main Menu":
                    return;
            }
        }
    }

    private void CreateCard(int deckId)
    {
        AnsiConsole.WriteLine();
        var word = AnsiConsole.Ask<string>("Enter card [green]word[/]:");

        var translation = AnsiConsole.Ask<string>("Enter card [green]translation[/]:");

        var result = _businessLogic.CreateFlashCard(word, translation, deckId);

        if (result.Success)
        {
            AnsiConsole.MarkupLine($"[green]✓ {result.Message}[/]");
        }
        else
        {
            AnsiConsole.MarkupLine($"[red]✗ {result.Message}[/]");
        }

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[grey]Press any key to continue...[/]");
        Console.ReadKey();
    }

    private void EditCard(int deckId)
    {
        var cards = _businessLogic.ListAllFlashCards(deckId);

        if (!cards.Any() || cards.All(c => c == null))
        {
            AnsiConsole.MarkupLine("[yellow]No cards to edit.[/]");
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[grey]Press any key to continue...[/]");
            Console.ReadKey();
            return;
        }

        var cardChoices = cards.Where(c => c != null).Select(c => $"{c!.Id} - {c.Word} - {c.Translation}").ToList();
        var selectedCard = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select a card to edit:")
                .AddChoices(cardChoices));

        var cardId = int.Parse(selectedCard.Split(" - ")[0]);

        AnsiConsole.WriteLine();
        var newWord = AnsiConsole.Ask<string>("Enter new [green]word[/]:");

        var newTranslation = AnsiConsole.Ask<string>("Enter new [green]translation[/]:");

        var result = _businessLogic.EditFlashCard(cardId, newWord, newTranslation);

        if (result.Success)
        {
            AnsiConsole.MarkupLine("[green]✓ Card updated successfully![/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[red]✗ Failed to update card.[/]");
        }

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[grey]Press any key to continue...[/]");
        Console.ReadKey();
    }

    private void DeleteCard(int deckId)
    {
        var cards = _businessLogic.ListAllFlashCards(deckId);

        if (!cards.Any() || cards.All(c => c == null))
        {
            AnsiConsole.MarkupLine("[yellow]No cards to delete.[/]");
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[grey]Press any key to continue...[/]");
            Console.ReadKey();
            return;
        }

        var cardChoices = cards.Where(c => c != null).Select(c => $"{c!.Id} - {c.Word} - {c.Translation}").ToList();
        var selectedCard = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select a card to delete:")
                .AddChoices(cardChoices));

        var cardId = int.Parse(selectedCard.Split(" - ")[0]);

        var confirm = AnsiConsole.Confirm("Are you sure you want to delete this card?");

        if (confirm)
        {
            var result = _businessLogic.DeleteFlashCard(cardId);

            if (result.Success)
            {
                AnsiConsole.MarkupLine("[green]✓ Card deleted successfully![/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[red]✗ Failed to delete card.[/]");
            }
        }
        else
        {
            AnsiConsole.MarkupLine("[yellow]Delete cancelled.[/]");
        }

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[grey]Press any key to continue...[/]");
        Console.ReadKey();
    }

    private void ShowDecks()
    {
        Console.Clear();
        AnsiConsole.MarkupLine("[bold blue]All Decks[/]");
        AnsiConsole.WriteLine();

        var decks = _businessLogic.ListAllDecks();

        if (!decks.Any() || decks.All(d => d == null))
        {
            AnsiConsole.MarkupLine("[yellow]No decks available.[/]");
            return;
        }

        var table = new Table();
        table.AddColumn("ID");
        table.AddColumn("Title");

        foreach (var deck in decks.Where(d => d != null))
        {
            table.AddRow(
                deck!.Id.ToString(),
                deck.Title);
        }

        AnsiConsole.Write(table);
    }

    private void StartStudyLesson()
    {
        Console.Clear();
        AnsiConsole.MarkupLine("[bold blue]Start Study Lesson[/]");
        AnsiConsole.WriteLine();

        var decks = _businessLogic.ListAllDecks();

        if (!decks.Any() || decks.All(d => d == null))
        {
            AnsiConsole.MarkupLine("[yellow]No decks available for study.[/]");
            return;
        }

        var deckChoices = decks.Where(d => d != null).Select(d => $"{d!.Id} - {d.Title}").ToList();
        var selectedDeck = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select a deck to study:")
                .AddChoices(deckChoices));

        var deckId = int.Parse(selectedDeck.Split(" - ")[0]);

        var shuffledCards = _businessLogic.GetShuffledCards(deckId);

        if (!shuffledCards.Any() || shuffledCards.All(c => c == null))
        {
            AnsiConsole.MarkupLine("[yellow]This deck has no cards to study.[/]");
            return;
        }

        AnsiConsole.MarkupLine("[green]Starting study session...[/]");
        AnsiConsole.WriteLine();

        var validCards = shuffledCards.Where(c => c != null).ToList();

        for (int i = 0; i < validCards.Count; i++)
        {
            var card = validCards[i]!;

            Console.Clear();
            AnsiConsole.MarkupLine($"[bold blue]Study Session - Card {i + 1} of {validCards.Count}[/]");
            AnsiConsole.WriteLine();

            var panel = new Panel(card.Word)
                .Header("Flash Card")
                .Border(BoxBorder.Rounded)
                .BorderColor(Color.Blue);

            AnsiConsole.Write(panel);
            AnsiConsole.WriteLine();

            AnsiConsole.MarkupLine("[grey]Press any key to continue to next card...[/]");
            Console.ReadKey();
        }

        Console.Clear();
        AnsiConsole.MarkupLine("[bold green]Study session completed! 🎉[/]");
        AnsiConsole.MarkupLine($"You studied [yellow]{validCards.Count}[/] cards.");
    }
}
