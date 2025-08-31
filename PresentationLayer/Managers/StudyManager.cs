using Business_Logic;
using Business_Logic.Dto;
using PresentationLayer.Helpers;
using Spectre.Console;

namespace PresentationLayer.Managers;

public class StudyManager
{
    private readonly BusinessLogic _businessLogic;

    public StudyManager(BusinessLogic businessLogic)
    {
        _businessLogic = businessLogic;
    }

    public void StartStudySession()
    {
        UIHelper.ShowSectionHeader("Start Study Lesson");

        var deckId = SelectDeckForStudy();
        if (deckId == null) return;

        var shuffledCards = _businessLogic.GetShuffledCards(deckId.Value);

        if (!shuffledCards.Any() || shuffledCards.All(c => c is null))
        {
            UIHelper.ShowWarning("This deck has no cards to study.");
            return;
        }

        var validCards = shuffledCards.Where(c => c is not null).Cast<FlashCardsDto>().ToList();
        var cardsToStudy = new List<FlashCardsDto>(validCards);
        var totalCards = validCards.Count;
        var totalAttempts = 0;
        var startTime = DateTime.Now;

        ShowStudyInstructions();

        while (cardsToStudy.Count > 0)
        {
            var currentCard = cardsToStudy[0];
            cardsToStudy.RemoveAt(0);

            var studyResult = ProcessCard(currentCard, cardsToStudy.Count, totalCards);

            if (studyResult.ShouldQuit)
            {
                ShowStudyCancelled();
                return;
            }

            totalAttempts++;

            if (studyResult.IsCorrect)
            {

            }
            else
            {
                cardsToStudy.Add(currentCard);
            }

            if (cardsToStudy.Count > 0)
            {
                UIHelper.WaitForKeyPress();
            }
        }

        var endTime = DateTime.Now;
        var duration = endTime - startTime;

        var saveResult = _businessLogic.SaveStudySession(deckId.Value, totalCards, totalAttempts, duration);

        ShowStudyComplete(totalCards, totalAttempts, duration, saveResult.Success);
    }

    private int? SelectDeckForStudy()
    {
        var decks = _businessLogic.ListAllDecks();

        if (!decks.Any() || decks.All(d => d == null))
        {
            UIHelper.ShowWarning("No decks available for study.");
            return null;
        }

        var deckChoices = decks.Where(d => d != null).Select(d => $"{d!.Id} - {d.Title}").ToList();
        var selectedDeck = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select a deck to study:")
                .AddChoices(deckChoices));

        return int.Parse(selectedDeck.Split(" - ")[0]);
    }

    private void ShowStudyInstructions()
    {
        AnsiConsole.MarkupLine("[green]Starting interactive study session...[/]");
        AnsiConsole.MarkupLine("[yellow]Type your translation for each word. You'll continue until you get all cards correct![/]");
        AnsiConsole.MarkupLine("[grey]Type 'quit' at any time to return to the main menu.[/]");
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[grey]Press any key to start...[/]");
        Console.ReadKey();
    }

    private StudyCardResult ProcessCard(FlashCardsDto currentCard, int remainingCards, int totalCards)
    {
        UIHelper.ClearScreen();
        
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
        var userAnswer = UIHelper.AskForInput("[green]Your translation[/] (or type 'quit' to exit):");

        // Check if user wants to quit
        if (userAnswer.Trim().ToLowerInvariant() == "quit")
        {
            return new StudyCardResult { ShouldQuit = true };
        }

        // Check if the answer is correct (case-insensitive)
        var isCorrect = string.Equals(userAnswer.Trim(), currentCard.Translation.Trim(), StringComparison.OrdinalIgnoreCase);

        AnsiConsole.WriteLine();

        if (isCorrect)
        {
            AnsiConsole.MarkupLine("[bold green]Correct! Well done![/]");
            
            // Show correct answer for confirmation
            AnsiConsole.MarkupLine($"[green]{currentCard.Word}[/] → [green]{currentCard.Translation}[/]");
            return new StudyCardResult { IsCorrect = true };
        }
        else
        {
            AnsiConsole.MarkupLine("[bold red]✗ Incorrect![/]");
            AnsiConsole.MarkupLine($"[red]Your answer:[/] {userAnswer}");
            AnsiConsole.MarkupLine($"[green]Correct answer:[/] {currentCard.Word} → [green]{currentCard.Translation}[/]");
            
            UIHelper.ShowWarning("This card will appear again later.");
            return new StudyCardResult { IsCorrect = false };
        }
    }

    private void ShowStudyCancelled()
    {
        UIHelper.ShowWarning("Study session cancelled. Returning to main menu...");
        UIHelper.WaitForKeyPress();
    }

    private void ShowStudyComplete(int totalCards, int totalAttempts, TimeSpan duration, bool sessionSaved)
    {
        UIHelper.ClearScreen();
        AnsiConsole.MarkupLine("[bold green]Study session completed![/]");
        AnsiConsole.WriteLine();
        
        AnsiConsole.MarkupLine($"[green]Total cards mastered:[/] [yellow]{totalCards}[/]");
        AnsiConsole.MarkupLine($"[green]Total attempts made:[/] [yellow]{totalAttempts}[/]");
        
        if (totalAttempts > totalCards)
        {
            AnsiConsole.MarkupLine($"[blue]You needed[/] [yellow]{totalAttempts - totalCards}[/] [blue]extra attempts to master all cards.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[green]Perfect! You got all cards right on the first try![/]");
        }

        var durationText = FormatDuration(duration);
        AnsiConsole.MarkupLine($"[green]Study duration:[/] [yellow]{durationText}[/]");

        if (sessionSaved)
        {
            UIHelper.ShowSuccess("Session saved to history!");
        }
        else
        {
            UIHelper.ShowWarning("Session completed but could not be saved to history.");
        }
    }

    private string FormatDuration(TimeSpan duration)
    {
        if (duration.TotalHours >= 1)
        {
            return $"{(int)duration.TotalHours:D2}:{duration.Minutes:D2}:{duration.Seconds:D2}";
        }
        else
        {
            return $"{duration.Minutes:D2}:{duration.Seconds:D2}";
        }
    }

    private class StudyCardResult
    {
        public bool IsCorrect { get; set; }
        public bool ShouldQuit { get; set; }
    }
}