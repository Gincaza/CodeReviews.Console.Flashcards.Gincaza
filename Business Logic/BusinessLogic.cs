using Business_Logic.DTO;
using Business_Logic.Interfaces;
using Business_Logic.Messages;
namespace Business_Logic;

public class BusinessLogic
{
    private IDataAccess dataAccess;

    public BusinessLogic(IDataAccess dataAccess)
    {
        this.dataAccess = dataAccess;
    }

    public OperationResult CreateDeck(string title)
    {
        bool success = dataAccess.createStacks(title);

        if (success)
        {
            return new OperationResult(success: true, message: "Deck created with success!");
        }

        return new OperationResult(success: false, message: "There was an error");
    }

    public OperationResult CreateFlashCard(string word, string translation, int stackId)
    {
        bool result = dataAccess.createFlashCard(word, translation, stackId);

        if (result)
        {
            return new OperationResult(success: true, message: "Card created with success!");
        }

        return new OperationResult(success: false, message: "There was an error.");
    }

    public OperationResult EditFlashCard(int cardId, string? word, string? translation)
    {
        bool result = dataAccess.updateFlashCard(
            cardId,
            word,
            translation);
        
        if (result)
        {
            return new OperationResult(true, null);
        }

        return new OperationResult(false, null);
    }

    public OperationResult DeleteFlashCard(int cardId)
    {
        bool result = dataAccess.deleteFlashCard(cardId);

        if (result)
        {
            return new OperationResult(true, null);
        }

        return new OperationResult(false, null);
    }

    public List<FlashCardsDTO?> ListAllFlashCards(int stackId)
    {
        List<FlashCardsDTO?> flashCardsDTOs = dataAccess.GetFlashCards(stackId);

        if (flashCardsDTOs != null && flashCardsDTOs.Count > 0)
        {
            return flashCardsDTOs;
        }
        
        return new List<FlashCardsDTO?>();
    }

    public List<StacksDTO?> ListAllDecks()
    {
        List<StacksDTO?> stacksDTOs = dataAccess.GetStacks();

        if (stacksDTOs != null && stacksDTOs.Count > 0) 
        { 
            return stacksDTOs; 
        }

        return new List<StacksDTO?>();
    }

    public List<FlashCardsDTO?> GetShuffledCards(int stackId)
    {
        List<FlashCardsDTO?> cardsList = dataAccess.GetFlashCards(stackId);

        Shuffle(cardsList);

        return cardsList;
    }

    public OperationResult DeleteStack(int stackId)
    {
        bool result = dataAccess.deleteStack(stackId);

        if (result)
        {
            return new OperationResult(true, null);
        }
        else
        {
            return new OperationResult(false, null);
        }
    }

    public OperationResult SaveStudySession(int stackId, int totalCards, int totalAttempts, TimeSpan duration)
    {
        bool result = dataAccess.InsertStudySession(stackId, totalCards, totalAttempts, duration);

        if (result)
        {
            return new OperationResult(true, "Study session saved successfully!");
        }
        else
        {
            return new OperationResult(false, "Failed to save study session.");
        }
    }

    public List<StudySessionDto?> GetStudySessions(int? stackId = null)
    {
        List<StudySessionDto?> studySessionDTOs = dataAccess.GetStudySession(stackId);

        if (studySessionDTOs != null && studySessionDTOs.Count > 0)
        {
            return studySessionDTOs;
        }

        return new List<StudySessionDto?>();
    }

    private void Shuffle(List<FlashCardsDTO?> cards)
    {
        int n = cards.Count;
        Random rng = new();

        while (n > 1)
        {
            int k = rng.Next(n);
            n--;
            (cards[n], cards[k]) = (cards[k], cards[n]);
        }
    }
}