using Business_Logic.DTO;
namespace Business_Logic.Interfaces;

public interface IDataAccess
{
    string configString { get; }

    List<StacksDTO?> GetStacks();
    List<FlashCardsDTO?> GetFlashCards(int stackId);
    bool createFlashCard(string description, int stackId);
    bool createStacks(string title);
    bool updateFlashCard(int cardId, string? description, int? stackId);
    bool deleteFlashCard(int cardId);
}