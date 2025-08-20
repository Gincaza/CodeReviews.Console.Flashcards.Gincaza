using Business_Logic.DTO;
namespace Business_Logic.Interfaces;

public interface IDataAccess
{
    string configString { get; }

    List<StacksDTO?> GetStacks();
    List<FlashCardsDTO?> GetFlashCards(int stackId);
    bool createFlashCard(string word,string translation, int stackId);
    bool createStacks(string title);
    bool updateFlashCard(int cardId, string? word, string? translation);
    bool deleteFlashCard(int cardId);
}