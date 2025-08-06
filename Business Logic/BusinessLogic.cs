using Business_Logic.DTO;
using Business_Logic.Interfaces;
using Business_Logic.Messages;
using System.Reflection.Metadata.Ecma335;

namespace Business_Logic;

class BusinessLogic
{
    private IDataAcess dataAccess;

    public BusinessLogic(IDataAcess dataAccess)
    {
        this.dataAccess = dataAccess;
    }

    public OperationResult CreateDeck(string title)
    {
        bool success = dataAccess.createStacks(title);

        if (success)
        {
            return new OperationResult(success: true, message: "Deck created with success!");
        }

        return new OperationResult(success: false, message: "There was an error");
    }

    public OperationResult CreateFlashCard(string description, int stackId)
    {
        bool result = dataAccess.createFlashCard(description, stackId);

        if (result)
        {
            return new OperationResult(success: true, message: "Card created with success!");
        }

        return new OperationResult(success: false, message: "There was an error.");
    }

    public OperationResult EditFlashCard(int cardId, string? description, int? stackId)
    {
        bool result = dataAccess.updateFlashCard(cardId, description, stackId);
        
        if (result)
        {
            return new OperationResult(true, null);
        }

        return new OperationResult(false, null);
    }

    public OperationResult DeleteFlashCard(int cardId)
    {
        bool result = dataAccess.deleteFlashCard(cardId);

        if (result)
        {
            return new OperationResult(true, null);
        }

        return new OperationResult(false, null);
    }

    public List<FlashCardsDTO?> ListAllFlashCards(int stackId)
    {
        List<FlashCardsDTO?> flashCardsDTOs = dataAccess.GetFlashCards(stackId);

        if (flashCardsDTOs != null && flashCardsDTOs.Count > 0)
        {
            return flashCardsDTOs;
        }
        
        return new List<FlashCardsDTO?>();
    }

    public List<StacksDTO?> ListAllDecks()
    {
        List<StacksDTO?> stacksDTOs = dataAccess.Stacks;

        if (stacksDTOs != null && stacksDTOs.Count > 0) 
        { 
            return stacksDTOs; 
        }

        return new List<StacksDTO?>();
    }

    public List<FlashCardsDTO?> GetShuffledCards(int stackId)
    {
        List<FlashCardsDTO?> cardsList = dataAccess.GetFlashCards(stackId);

        Shuffle(cardsList);

        return cardsList;
    }

    private void Shuffle(List<FlashCardsDTO?> cards)
    {
        int n = cards.Count;
        Random rng = new();

        while (n > 1)
        {
            int k = rng.Next(n);
            n--;
            (cards[n], cards[k]) = (cards[k], cards[n]);
        }
    }
}