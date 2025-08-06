using Business_Logic.Interfaces;
using Business_Logic.DTO;

namespace DataAccess;

public class DataAccessRepository : IDataAcess
{
    public string configString => throw new NotImplementedException();
    public List<FlashCardsDTO?> FlashCards
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public List<StacksDTO?> Stacks
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public bool addCodingSession()
    {
        throw new NotImplementedException();
    }

    public bool createFlashCard(string description, int stackId)
    {
        throw new NotImplementedException();
    }

    public bool createStacks(string title)
    {
        throw new NotImplementedException();
    }

    public bool updateFlashCard(int cardId, string? description, int? stackId)
    {
        throw new NotImplementedException();
    }
}
 