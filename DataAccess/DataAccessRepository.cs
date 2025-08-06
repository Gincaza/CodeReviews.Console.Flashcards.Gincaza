using Business_Logic.Interfaces;
using Business_Logic.DTO;

namespace DataAccess;

public class DataAccessRepository : IDataAcess
{
    public string configString => throw new NotImplementedException();
    public List<StacksDTO> FlashCards => throw new NotImplementedException();
    public List<StacksDTO> Stacks => throw new NotImplementedException();

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
}
 