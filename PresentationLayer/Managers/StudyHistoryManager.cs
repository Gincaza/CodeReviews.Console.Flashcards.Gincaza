using Business_Logic;
using Business_Logic.Dto;
using PresentationLayer.Helpers;
using Spectre.Console;

namespace PresentationLayer.Managers;

public class StudyHistoryManager
{
    private readonly BusinessLogic _businessLogic;

    public StudyHistoryManager(BusinessLogic businessLogic)
    {
        _businessLogic = businessLogic;
    }

    public void ShowStudyHistory()
    {
        UIHelper.ShowSectionHeader("Study History");

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("What would you like to view?")
                .AddChoices(new[]
                {
                    "All Study Sessions",
                    "Study Sessions by Deck",
                    "Back to Main Menu"
                }));

        switch (choice)
        {
            case "All Study Sessions":
                ShowAllStudySessions();
                break;
            case "Study Sessions by Deck":
                ShowStudySessionsByDeck();
                break;
            case "Back to Main Menu":
                return;
        }
    }

    private void ShowAllStudySessions()
    {
        UIHelper.ShowSectionHeader("All Study Sessions");

        var sessions = _businessLogic.GetStudySessions();

        if (!sessions.Any() || sessions.All(s => s == null))
        {
            UIHelper.ShowWarning("No study sessions found.");
            return;
        }

        DisplayStudySessionsTable(sessions.Where(s => s != null).ToList()!);
    }

    private void ShowStudySessionsByDeck()
    {
        UIHelper.ShowSectionHeader("Study Sessions by Deck");

        var decks = _businessLogic.ListAllDecks();

        if (!decks.Any() || decks.All(d => d == null))
        {
            UIHelper.ShowWarning("No decks available.");
            return;
        }

        var deckChoices = decks.Where(d => d != null).Select(d => $"{d!.Id} - {d.Title}").ToList();
        var selectedDeck = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select a deck to view study history:")
                .AddChoices(deckChoices));

        var deckId = int.Parse(selectedDeck.Split(" - ")[0]);
        var deckTitle = selectedDeck.Split(" - ")[1];

        var sessions = _businessLogic.GetStudySessions(deckId);

        if (!sessions.Any() || sessions.All(s => s == null))
        {
            UIHelper.ShowWarning($"No study sessions found for deck '{deckTitle}'.");
            return;
        }

        AnsiConsole.MarkupLine($"[bold blue]Study History for deck: {deckTitle}[/]");
        AnsiConsole.WriteLine();

        DisplayStudySessionsTable(sessions.Where(s => s != null).ToList()!);
    }

    private void DisplayStudySessionsTable(List<StudySessionDto> sessions)
    {
        var table = new Table();
        table.AddColumn("Date");
        table.AddColumn("Deck ID");
        table.AddColumn("Cards");
        table.AddColumn("Attempts");
        table.AddColumn("Extra");
        table.AddColumn("Duration");
        table.AddColumn("Accuracy");
        table.AddColumn("Perfect");

        foreach (var session in sessions)
        {
            var accuracy = session.AccuracyPercentage.ToString("F1") + "%";
            var perfect = session.PerfectSession ? "✓" : "✗";
            var duration = FormatDuration(session.StudyDuration);

            table.AddRow(
                session.SessionDate.ToString("dd/MM HH:mm"),
                session.StackId.ToString(),
                session.TotalCards.ToString(),
                session.TotalAttempts.ToString(),
                session.ExtraAttempts.ToString(),
                duration,
                accuracy,
                perfect
            );
        }

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();

        ShowStudyStatistics(sessions);
    }

    private void ShowStudyStatistics(List<StudySessionDto> sessions)
    {
        if (!sessions.Any()) return;

        var totalSessions = sessions.Count;
        var perfectSessions = sessions.Count(s => s.PerfectSession);
        var averageAccuracy = sessions.Average(s => s.AccuracyPercentage);
        var totalStudyTime = TimeSpan.FromMilliseconds(sessions.Sum(s => s.StudyDuration.TotalMilliseconds));
        var averageSessionTime = TimeSpan.FromMilliseconds(sessions.Average(s => s.StudyDuration.TotalMilliseconds));

        AnsiConsole.MarkupLine("[bold yellow]📊 Study Statistics[/]");
        AnsiConsole.WriteLine();

        var statsTable = new Table();
        statsTable.AddColumn("Metric");
        statsTable.AddColumn("Value");

        statsTable.AddRow("Total Sessions", totalSessions.ToString());
        statsTable.AddRow("Perfect Sessions", $"{perfectSessions} ({(double)perfectSessions / totalSessions * 100:F1}%)");
        statsTable.AddRow("Average Accuracy", $"{averageAccuracy:F1}%");
        statsTable.AddRow("Total Study Time", FormatDuration(totalStudyTime));
        statsTable.AddRow("Average Session Time", FormatDuration(averageSessionTime));

        AnsiConsole.Write(statsTable);
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
}