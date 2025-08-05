using Business_Logic.Interfaces;

namespace DataAccess;

public class DataAccessRepository : IDataAcess
{
    public string configString => throw new NotImplementedException();

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

    public bool getFlashCards()
    {
        throw new NotImplementedException();
    }

    public bool getStacks()
    {
        throw new NotImplementedException();
    }
}
 