using Business_Logic.Dto;
namespace Business_Logic.Interfaces;

public interface IDataAccess
{
    string configString { get; }

    List<StacksDto?> GetStacks();
    List<FlashCardsDto?> GetFlashCards(int stackId);
    bool CreateFlashCard(string word,string translation, int stackId);
    bool CreateStacks(string title);
    bool updateFlashCard(int cardId, string? word, string? translation);
    bool deleteFlashCard(int cardId);
    bool deleteStack(int stackId);
    bool InsertStudySession(int stackId, int totalCards, int totalAttempts, TimeSpan duration);
    List<StudySessionDto?> GetStudySession(int? stackId = null);
}