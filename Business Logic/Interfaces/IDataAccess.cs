using Business_Logic.Dto;
namespace Business_Logic.Interfaces;

public interface IDataAccess
{
    string ConfigString { get; }

    List<StacksDto?> GetStacks();
    List<FlashCardsDto?> GetFlashCards(int stackId);
    bool CreateFlashCard(string word,string translation, int stackId);
    bool CreateStacks(string title);
    bool UpdateFlashCard(int cardId, string? word, string? translation);
    bool DeleteFlashCard(int cardId);
    bool DeleteStack(int stackId);
    bool InsertStudySession(int stackId, int totalCards, int totalAttempts, TimeSpan duration);
    List<StudySessionDto?> GetStudySession(int? stackId = null);
}