using Business_Logic.Interfaces;
using Business_Logic.DTO;
using Dapper;
using Microsoft.Data.SqlClient;

namespace DataAccess;

public class DataAccessRepository : IDataAccess
{
    public string configString => "Data Source=localhost;Initial Catalog=FlashcardsDB;Integrated Security=true;TrustServerCertificate=true;";

    public DataAccessRepository()
    {
        InitializeDatabase();
    }

    public List<FlashCardsDTO?> GetFlashCards(int stackId)
    {
        throw new NotImplementedException();
    }

    private void InitializeDatabase()
    {
        using var connection = new SqlConnection(configString);
        connection.Open();

        string sql = @"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Stacks' AND xtype='U')
            BEGIN
                CREATE TABLE Stacks (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    Title NVARCHAR(100) NOT NULL
                );
            END;

            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='FlashCards' AND xtype='U')
            BEGIN
                CREATE TABLE FlashCards (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    Description NVARCHAR(255) NOT NULL,
                    Stack INT NOT NULL,
                    CONSTRAINT FK_FlashCards_Stacks FOREIGN KEY (Stack)
                        REFERENCES Stacks(Id)
                        ON DELETE CASCADE
                );
            END;
        ";

        connection.Execute(sql);
    }

    public List<StacksDTO?> GetStacks()
    {
        try
        {
            using var connection = new SqlConnection(configString);

            return connection.Query<StacksDTO>(
                "SELECT Id, Title FROM Stacks ORDER BY Title").Cast<StacksDTO?>().ToList();
        }
        catch (Exception)
        {
            return new List<StacksDTO?>();
        }
    }

    public bool createFlashCard(string description, int stackId)
    {
        try
        {
            using var connection = new SqlConnection(configString);

            var stackExists = connection.QuerySingleOrDefault<int?>(
                "SELECT COUNT(2) FROM Stacks WHERE Id = @StackId",
                new { StackId = stackId });

            if (stackExists == 0)
                return false;


            string sql = @"INSERT INTO FlashCards (Description, Stack) VALUES (@Description, @Stack)";
            var rowsAffected = connection.Execute(sql, new { Description = description, Stack =  stackId });

            return rowsAffected > 0;
        }
        catch (Exception)
        {
            return false;
        }
    }
    
    public bool createStacks(string title)
    {
        if (string.IsNullOrEmpty(title)) 
            return false;

        if (title.Length > 100) 
            return false;

        try
        {
            using var connection = new SqlConnection(configString);

            var existingCount = connection.QuerySingleOrDefault<int>(
                "SELECT COUNT(1) FROM Stacks WHERE Title = @Title",
                new { Title = title });

            if (existingCount > 0)
                return false;

            string sql = @"INSERT INTO Stacks (Title) VALUES (@Title)";
            var rowsAffected = connection.Execute(sql, new { Title = title });

            return rowsAffected > 0;
        }
        catch (Exception)
        {
            return false;
        }
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
 