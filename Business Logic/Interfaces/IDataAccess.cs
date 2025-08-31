using Business_Logic.Dto;
namespace Business_Logic.Interfaces;

public interface IDataAccess
{
    string configString { get; }

    List<StacksDTO?> GetStacks();
    List<FlashCardsDto?> GetFlashCards(int stackId);
    bool createFlashCard(string word,string translation, int stackId);
    bool createStacks(string title);
    bool updateFlashCard(int cardId, string? word, string? translation);
    bool deleteFlashCard(int cardId);
    bool deleteStack(int stackId);
}