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
                        "Delete Deck",
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
                case "Delete Deck":
                    if (DeleteDeck(deckId))
                        return; // Exit to main menu if deck was deleted
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

    private bool DeleteDeck(int deckId)
    {
        var decks = _businessLogic.ListAllDecks();
        var deck = decks.FirstOrDefault(d => d != null && d.Id == deckId);
        
        if (deck == null)
        {
            AnsiConsole.MarkupLine("[red]✗ Deck not found.[/]");
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[grey]Press any key to continue...[/]");
            Console.ReadKey();
            return false;
        }

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine($"[bold red]⚠️  Warning: You are about to delete the deck '[yellow]{deck.Title}[/]'[/]");
        AnsiConsole.MarkupLine("[red]This will also delete all cards in this deck and cannot be undone![/]");
        AnsiConsole.WriteLine();

        var confirm = AnsiConsole.Confirm("Are you sure you want to delete this deck?");

        if (confirm)
        {
            var result = _businessLogic.DeleteStack(deckId);

            if (result.Success)
            {
                AnsiConsole.MarkupLine("[green]✓ Deck deleted successfully![/]");
                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine("[grey]Press any key to continue...[/]");
                Console.ReadKey();
                return true; // Deck was deleted, exit to main menu
            }
            else
            {
                AnsiConsole.MarkupLine("[red]✗ Failed to delete deck.[/]");
            }
        }
        else
        {
            AnsiConsole.MarkupLine("[yellow]Delete cancelled.[/]");
        }

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[grey]Press any key to continue...[/]");
        Console.ReadKey();
        return false; // Deck was not deleted, stay in manage menu
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

        var validCards = shuffledCards.Where(c => c != null).ToList();
        var cardsToStudy = new List<FlashCardsDTO?>(validCards);
        var totalCards = validCards.Count;
        var correctAnswers = 0;

        AnsiConsole.MarkupLine("[green]Starting interactive study session...[/]");
        AnsiConsole.MarkupLine("[yellow]Type your translation for each word. You'll continue until you get all cards correct![/]");
        AnsiConsole.MarkupLine("[grey]Type 'quit' at any time to return to the main menu.[/]");
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[grey]Press any key to start...[/]");
        Console.ReadKey();

        while (cardsToStudy.Count > 0)
        {
            var currentCard = cardsToStudy[0]!;
            cardsToStudy.RemoveAt(0);

            Console.Clear();
            
            var remainingCards = cardsToStudy.Count;
            var completedCards = totalCards - remainingCards - 1;
            
            AnsiConsole.MarkupLine($"[bold blue]Study Session - Progress: {completedCards}/{totalCards} completed[/]");
            AnsiConsole.MarkupLine($"[grey]Cards remaining: {remainingCards + 1}[/]");
            AnsiConsole.WriteLine();

            // Show the word to translate
            var wordPanel = new Panel(currentCard.Word)
                .Header("Translate this word")
                .Border(BoxBorder.Rounded)
                .BorderColor(Color.Blue);

            AnsiConsole.Write(wordPanel);
            AnsiConsole.WriteLine();

            // Get user's translation
            var userAnswer = AnsiConsole.Ask<string>("[green]Your translation[/] (or type 'quit' to exit):");

            // Check if user wants to quit
            if (userAnswer.Trim().ToLowerInvariant() == "quit")
            {
                AnsiConsole.MarkupLine("[yellow]Study session cancelled. Returning to main menu...[/]");
                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine("[grey]Press any key to continue...[/]");
                Console.ReadKey();
                return;
            }

            // Check if the answer is correct (case-insensitive)
            var isCorrect = string.Equals(userAnswer.Trim(), currentCard.Translation.Trim(), StringComparison.OrdinalIgnoreCase);

            AnsiConsole.WriteLine();

            if (isCorrect)
            {
                AnsiConsole.MarkupLine("[bold green]✓ Correct! Well done![/]");
                correctAnswers++;
                
                // Show correct answer for confirmation
                AnsiConsole.MarkupLine($"[green]{currentCard.Word}[/] → [green]{currentCard.Translation}[/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[bold red]✗ Incorrect![/]");
                AnsiConsole.MarkupLine($"[red]Your answer:[/] {userAnswer}");
                AnsiConsole.MarkupLine($"[green]Correct answer:[/] {currentCard.Word} → [green]{currentCard.Translation}[/]");
                
                // Add the card back to the end of the queue to try again
                cardsToStudy.Add(currentCard);
                AnsiConsole.MarkupLine("[yellow]This card will appear again later.[/]");
            }

            AnsiConsole.WriteLine();
            
            if (cardsToStudy.Count > 0)
            {
                AnsiConsole.MarkupLine("[grey]Press any key to continue to the next card...[/]");
                Console.ReadKey();
            }
        }

        // Study session completed
        Console.Clear();
        AnsiConsole.MarkupLine("[bold green]🎉 Study session completed! 🎉[/]");
        AnsiConsole.WriteLine();
        
        AnsiConsole.MarkupLine($"[green]Total cards mastered:[/] [yellow]{totalCards}[/]");
        AnsiConsole.MarkupLine($"[green]Total attempts made:[/] [yellow]{correctAnswers}[/]");
    }
}
