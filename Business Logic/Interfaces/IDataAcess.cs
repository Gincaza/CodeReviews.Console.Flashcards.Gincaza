using Business_Logic.DTO;
namespace Business_Logic.Interfaces;

public interface IDataAcess
{
    string configString { get; }
    List<StacksDTO?> Stacks { get; }

    List<FlashCardsDTO?> GetFlashCards(int stackId);
    bool createFlashCard(string description, int stackId);
    bool createStacks(string title);
    bool updateFlashCard(int cardId, string? description, int? stackId);
    bool deleteFlashCard(int cardId);
}