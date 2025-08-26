using Business_Logic;
using Business_Logic.DTO;
using PresentationLayer.Helpers;
using Spectre.Console;

namespace PresentationLayer.Managers;

public class DeckManager
{
    private readonly BusinessLogic _businessLogic;

    public DeckManager(BusinessLogic businessLogic)
    {
        _businessLogic = businessLogic;
    }

    public void CreateDeck()
    {
        UIHelper.ShowSectionHeader("Create New Deck");

        var title = UIHelper.AskForInput("Enter deck [green]title[/]:");

        AnsiConsole.Status()
            .Start("Creating deck...", ctx =>
            {
                var result = _businessLogic.CreateDeck(title);

                if (result.Success)
                {
                    UIHelper.ShowSuccess(result.Message ?? "Deck created successfully!");
                }
                else
                {
                    UIHelper.ShowError(result.Message ?? "Failed to create deck");
                }
            });
    }

    public void ShowAllDecks()
    {
        UIHelper.ShowSectionHeader("All Decks");

        var decks = _businessLogic.ListAllDecks();

        if (!decks.Any() || decks.All(d => d == null))
        {
            UIHelper.ShowWarning("No decks available.");
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

    public int? SelectDeck(string title = "Select a deck:")
    {
        var decks = _businessLogic.ListAllDecks();

        if (!decks.Any() || decks.All(d => d == null))
        {
            UIHelper.ShowWarning("No decks available. Please create a deck first.");
            return null;
        }

        var deckChoices = decks.Where(d => d != null).Select(d => $"{d!.Id} - {d.Title}").ToList();
        var selectedDeck = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title(title)
                .AddChoices(deckChoices));

        return int.Parse(selectedDeck.Split(" - ")[0]);
    }

    public bool DeleteDeck(int deckId)
    {
        var decks = _businessLogic.ListAllDecks();
        var deck = decks.FirstOrDefault(d => d != null && d.Id == deckId);
        
        if (deck == null)
        {
            UIHelper.ShowError("Deck not found.");
            UIHelper.WaitForKeyPress();
            return false;
        }

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine($"[bold red]Warning: You are about to delete the deck '[yellow]{deck.Title}[/]'[/]");
        AnsiConsole.MarkupLine("[red]This will also delete all cards in this deck and cannot be undone![/]");
        AnsiConsole.WriteLine();

        var confirm = UIHelper.ConfirmAction("Are you sure you want to delete this deck?");

        if (confirm)
        {
            var result = _businessLogic.DeleteStack(deckId);

            if (result.Success)
            {
                UIHelper.ShowSuccess("Deck deleted successfully!");
                UIHelper.WaitForKeyPress();
                return true; // Deck was deleted, exit to main menu
            }
            else
            {
                UIHelper.ShowError("Failed to delete deck.");
            }
        }
        else
        {
            UIHelper.ShowWarning("Delete cancelled.");
        }

        UIHelper.WaitForKeyPress();
        return false; // Deck was not deleted, stay in manage menu
    }

    public void ShowDeckCards(int deckId)
    {
        var cards = _businessLogic.ListAllFlashCards(deckId);

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
            UIHelper.ShowWarning("No cards in this deck yet.");
            AnsiConsole.WriteLine();
        }
    }
}