using Business_Logic.DTO;
namespace Business_Logic.Interfaces;

public interface IDataAccess
{
    string configString { get; }

    List<StacksDTO?> GetStacks();
    List<FlashCardsDTO?> GetFlashCards(int stackId);
    bool createFlashCard(string description,string translation, int stackId);
    bool createStacks(string title);
    bool updateFlashCard(int cardId, string? description, string? translation);
    bool deleteFlashCard(int cardId);
}