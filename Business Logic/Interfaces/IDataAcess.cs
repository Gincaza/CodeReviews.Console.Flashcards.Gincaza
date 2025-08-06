using Business_Logic.DTO;
namespace Business_Logic.Interfaces;

public interface IDataAcess
{
    string configString { get; }
    List<StacksDTO> Stacks { get; }
    List<StacksDTO> FlashCards { get; }

    bool createFlashCard(string description, int stackId);
    bool createStacks(string title);
}