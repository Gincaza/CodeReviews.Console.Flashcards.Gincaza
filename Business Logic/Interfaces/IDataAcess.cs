namespace Business_Logic.Interfaces;

public interface IDataAcess
{
    string configString { get; }
    bool getStacks();
    bool getFlashCards();
    bool createFlashCard(string description, int stackId);
    bool createStacks(string title);
}