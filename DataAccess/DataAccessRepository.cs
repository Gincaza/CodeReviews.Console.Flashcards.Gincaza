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
        try
        {
            using var connection = new SqlConnection(configString);

            var flashCards = connection.Query<FlashCardsDTO>(
                "SELECT Id, Word, Stack, Translation FROM FlashCards WHERE Stack = @StackId",
                new { StackId = stackId }).ToList();

            return flashCards.Cast<FlashCardsDTO?>().ToList();
        }
        catch (Exception)
        {
            return new List<FlashCardsDTO?>();
        }
    }

    private void InitializeDatabase()
    {
        var masterConnectionString = "Data Source=localhost;Initial Catalog=master;Integrated Security=true;TrustServerCertificate=true;";

        using (var connection = new SqlConnection(masterConnectionString))
        {
            string createDbSql = @"
        IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'FlashcardsDB')
        BEGIN
            CREATE DATABASE FlashcardsDB;
        END";

            connection.Execute(createDbSql);
        }

        using var dbConnection = new SqlConnection(configString);

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
                Word NVARCHAR(255) NOT NULL,
                Stack INT NOT NULL,
                Translation NVARCHAR(255) NOT NULL,
                CONSTRAINT FK_FlashCards_Stacks FOREIGN KEY (Stack)
                    REFERENCES Stacks(Id)
                    ON DELETE CASCADE
            );
        END;
    ";

        dbConnection.Execute(sql);
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

    public bool createFlashCard(string description, string translation, int stackId)
    {
        try
        {
            using var connection = new SqlConnection(configString);

            var stackExists = connection.QuerySingleOrDefault<int?>(
                "SELECT COUNT(2) FROM Stacks WHERE Id = @StackId",
                new { StackId = stackId });

            if (stackExists == 0)
                return false;


            string sql = @"INSERT INTO FlashCards (Description, Translation, Stack) VALUES (@Description, @Translation, @Stack)";
            var rowsAffected = connection.Execute(sql, new { Description = description, Translation = translation, Stack =  stackId });

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

    public bool updateFlashCard(int cardId, string? description, string? translation)
    {
        using var connection = new SqlConnection(this.configString);

        string query;
        object parameters;

        if (!string.IsNullOrEmpty(description) && !string.IsNullOrEmpty(translation))
        {
            query = "UPDATE FlashCards SET Description = @Description, Translation = @Translation WHERE Id = @Id";
            parameters = new { Description = description, Translation = translation, Id = cardId };
        }
        else if (!string.IsNullOrEmpty(description))
        {
            query = "UPDATE FlashCards SET Description = @Description WHERE Id = @Id";
            parameters = new { Description = description, Id = cardId };
        }

        else if (!string.IsNullOrEmpty(translation))
        {
            query = "UPDATE FlashCards SET Translation = @Translation WHERE Id = @Id";
            parameters = new { Translation = translation, Id = cardId };
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
        try
        {
            using var connection = new SqlConnection(configString);
            var rowsAffected = connection.Execute("DELETE FROM FlashCards WHERE Id = @Id", new { Id = cardId });
            return rowsAffected > 0;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
 