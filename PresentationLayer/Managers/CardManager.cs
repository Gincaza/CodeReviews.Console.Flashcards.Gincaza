using Business_Logic;
using Business_Logic.Dto;
using PresentationLayer.Helpers;
using Spectre.Console;

namespace PresentationLayer.Managers;

public class CardManager
{
    private readonly BusinessLogic _businessLogic;

    public CardManager(BusinessLogic businessLogic)
    {
        _businessLogic = businessLogic;
    }

    public void CreateCard(int deckId)
    {
        AnsiConsole.WriteLine();
        var word = UIHelper.AskForInput("Enter card [green]word[/]:");
        var translation = UIHelper.AskForInput("Enter card [green]translation[/]:");

        var result = _businessLogic.CreateFlashCard(word, translation, deckId);

        if (result.Success)
        {
            UIHelper.ShowSuccess(result.Message ?? "Card created successfully!");
        }
        else
        {
            UIHelper.ShowError(result.Message ?? "Failed to create card");
        }

        UIHelper.WaitForKeyPress();
    }

    public void EditCard(int deckId)
    {
        var cardId = SelectCard(deckId, "Select a card to edit:");
        if (cardId == null) return;

        AnsiConsole.WriteLine();
        var newWord = UIHelper.AskForInput("Enter new [green]word[/]:");
        var newTranslation = UIHelper.AskForInput("Enter new [green]translation[/]:");

        var result = _businessLogic.EditFlashCard(cardId.Value, newWord, newTranslation);

        if (result.Success)
        {
            UIHelper.ShowSuccess("Card updated successfully!");
        }
        else
        {
            UIHelper.ShowError("Failed to update card.");
        }

        UIHelper.WaitForKeyPress();
    }

    public void DeleteCard(int deckId)
    {
        var cardId = SelectCard(deckId, "Select a card to delete:");
        if (cardId == null) return;

        var confirm = UIHelper.ConfirmAction("Are you sure you want to delete this card?");

        if (confirm)
        {
            var result = _businessLogic.DeleteFlashCard(cardId.Value);

            if (result.Success)
            {
                UIHelper.ShowSuccess("Card deleted successfully!");
            }
            else
            {
                UIHelper.ShowError("Failed to delete card.");
            }
        }
        else
        {
            UIHelper.ShowWarning("Delete cancelled.");
        }

        UIHelper.WaitForKeyPress();
    }

    private int? SelectCard(int deckId, string title)
    {
        var cards = _businessLogic.ListAllFlashCards(deckId);

        if (!cards.Any() || cards.All(c => c == null))
        {
            UIHelper.ShowWarning("No cards available for this operation.");
            UIHelper.WaitForKeyPress();
            return null;
        }

        var cardChoices = cards.Where(c => c != null).Select(c => $"{c!.Id} - {c.Word} - {c.Translation}").ToList();
        var selectedCard = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title(title)
                .AddChoices(cardChoices));

        return int.Parse(selectedCard.Split(" - ")[0]);
    }
}