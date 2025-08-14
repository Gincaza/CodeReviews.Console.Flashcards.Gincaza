using Business_Logic.Interfaces;
using Business_Logic.DTO;
using Dapper;
using Microsoft.Data.SqlClient;

namespace DataAccess;

public class DataAccessRepository : IDataAcess
{
    public string configString => throw new NotImplementedException();
    public List<FlashCardsDTO?> GetFlashCards(int stackId)
    {
        throw new NotImplementedException();
    }

    private void InitializeDatabase()
    {
        //using var connection = new SqliteConn
    }

    public List<StacksDTO?> Stacks
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public bool addCodingSession()
    {
        throw new NotImplementedException();
    }

    public bool createFlashCard(string description, int stackId)
    {
        throw new NotImplementedException();
    }

    public bool createStacks(string title)
    {
        throw new NotImplementedException();
    }

    public bool updateFlashCard(int cardId, string? description, int? stackId)
    {
        using var connection = new SqlConnection(this.configString);

        string query;
        object parameters;

        if (stackId.HasValue && !string.IsNullOrEmpty(description))
        {
            query = "UPDATE FlashCards SET Description = @Description, Stack = @Stack WHERE Id = @Id";
            parameters = new { Description = description, Stack = stackId.Value, Id = cardId };
        }
        else if (!string.IsNullOrEmpty(description))
        {
            query = "UPDATE FlashCards SET Description = @Description WHERE Id = @Id";
            parameters = new { Description = description, Id = cardId };
        }
        else if (stackId.HasValue)
        {
            query = "UPDATE FlashCards SET Stack = @Stack WHERE Id = @Id";
            parameters = new { Stack = stackId.Value, Id = cardId };
        }
        else
        {
            return false;
        }

        var rowsAffected = connection.Execute(query, parameters);
        return rowsAffected > 0;
    }

    public bool deleteFlashCard(int cardId)
    {
        throw new NotImplementedException();
    }
}
 