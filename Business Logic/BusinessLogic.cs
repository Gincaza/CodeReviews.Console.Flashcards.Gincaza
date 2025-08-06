using Business_Logic.DTO;
using Business_Logic.Interfaces;
using Business_Logic.Messages;

namespace Business_Logic;

class BusinessLogic
{
    private IDataAcess dataAccess;

    public BusinessLogic(IDataAcess dataAccess)
    {
        this.dataAccess = dataAccess;
    }

    public OperationResult createDeck(string title)
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

    public List<StacksDTO?> ListAllDecks()
    {
        List<StacksDTO?> stacksDTOs = dataAccess.Stacks;

        if (stacksDTOs != null && stacksDTOs.Count > 0) 
        { 
            return stacksDTOs; 
        }

        return new List<StacksDTO?>();
    }
}