using Business_Logic;
using PresentationLayer.Helpers;
using PresentationLayer.Managers;

namespace PresentationLayer;

public class FlashcardsUI
{
    private readonly BusinessLogic _businessLogic;
    private readonly MenuManager _menuManager;
    private readonly DeckManager _deckManager;
    private readonly CardManager _cardManager;
    private readonly StudyManager _studyManager;

    public FlashcardsUI(BusinessLogic businessLogic)
    {
        _businessLogic = businessLogic;
        _menuManager = new MenuManager();
        _deckManager = new DeckManager(businessLogic);
        _cardManager = new CardManager(businessLogic);
        _studyManager = new StudyManager(businessLogic);
    }

    public void Run()
    {
        _menuManager.ShowWelcomeBanner();

        while (true)
        {
            var choice = _menuManager.ShowMainMenu();

            try
            {
                switch (choice)
                {
                    case "Create deck":
                        _deckManager.CreateDeck();
                        break;
                    case "Manage Deck":
                        ManageDeck();
                        break;
                    case "See Decks":
                        _deckManager.ShowAllDecks();
                        break;
                    case "Start Study Lesson":
                        _studyManager.StartStudySession();
                        break;
                    case "Exit":
                        _menuManager.ShowGoodbye();
                        return;
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Error: {ex.Message}");
            }

            UIHelper.WaitForKeyPress();
            UIHelper.ClearScreen();
        }
    }

    private void ManageDeck()
    {
        UIHelper.ShowSectionHeader("Manage Deck");

        var deckId = _deckManager.SelectDeck();
        if (deckId == null) return;

        ManageDeckCards(deckId.Value);
    }

    private void ManageDeckCards(int deckId)
    {
        while (true)
        {
            UIHelper.ShowSectionHeader($"Managing Deck (ID: {deckId})");
            
            _deckManager.ShowDeckCards(deckId);

            var action = _menuManager.ShowManageDeckMenu();

            switch (action)
            {
                case "Create Card":
                    _cardManager.CreateCard(deckId);
                    break;
                case "Edit Card":
                    _cardManager.EditCard(deckId);
                    break;
                case "Delete Card":
                    _cardManager.DeleteCard(deckId);
                    break;
                case "Delete Deck":
                    if (_deckManager.DeleteDeck(deckId))
                        return; // Exit to main menu if deck was deleted
                    break;
                case "Back to Main Menu":
                    return;
            }
        }
    }
}
