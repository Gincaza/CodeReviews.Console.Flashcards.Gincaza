using Business_Logic.Interfaces;
using Business_Logic.Dto;
using Dapper;
using Microsoft.Data.SqlClient;

namespace DataAccess;

public class DataAccessRepository : IDataAccess
{
    public string ConfigString => "Data Source=localhost;Initial Catalog=FlashcardsDB;Integrated Security=true;TrustServerCertificate=true;";

    public DataAccessRepository()
    {
        InitializeDatabase();
    }

    public List<FlashCardsDto?> GetFlashCards(int stackId)
    {
        try
        {
            using var connection = new SqlConnection(ConfigString);

            var flashCards = connection.Query<FlashCardsDto>(
                "SELECT Id, Word, Stack, Translation FROM FlashCards WHERE Stack = @StackId",
                new { StackId = stackId }).ToList();

            return flashCards.Cast<FlashCardsDto?>().ToList();
        }
        catch (Exception)
        {
            return new List<FlashCardsDto?>();
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

        using var dbConnection = new SqlConnection(ConfigString);

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
        
        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='StudySessions' AND xtype='U')
        BEGIN
            CREATE TABLE StudySessions (
                Id INT IDENTITY(1,1) PRIMARY KEY,
                StackId INT NOT NULL,
                SessionDate DATETIME2 NOT NULL DEFAULT GETDATE(),
                TotalCards INT NOT NULL,
                TotalAttempts INT NOT NULL,
                CorrectAnswers INT NOT NULL,
                StudyDuration TIME NOT NULL,
                CONSTRAINT FK_StudySessions_Stacks FOREIGN KEY (StackId)
                    REFERENCES Stacks(Id)
                    ON DELETE CASCADE,
                CONSTRAINT CHK_TotalAttempts CHECK (TotalAttempts >= TotalCards),
                CONSTRAINT CHK_CorrectAnswers CHECK (CorrectAnswers = TotalCards),
                CONSTRAINT CHK_PositiveValues CHECK (TotalCards > 0 AND TotalAttempts > 0)
            );
        END;
    ";

        dbConnection.Execute(sql);
    }

    public List<StacksDto?> GetStacks()
    {
        try
        {
            using var connection = new SqlConnection(ConfigString);

            return connection.Query<StacksDto>(
                "SELECT Id, Title FROM Stacks ORDER BY Title").Cast<StacksDto?>().ToList();
        }
        catch (Exception)
        {
            return new List<StacksDto?>();
        }
    }

    public bool CreateFlashCard(string word, string translation, int stackId)
    {
        try
        {
            using var connection = new SqlConnection(ConfigString);

            var stackExists = connection.QuerySingleOrDefault<int?>(
                "SELECT COUNT(2) FROM Stacks WHERE Id = @StackId",
                new { StackId = stackId });

            if (stackExists == 0)
                return false;


            string sql = @"INSERT INTO FlashCards (Word, Translation, Stack) VALUES (@Word, @Translation, @Stack)";
            var rowsAffected = connection.Execute(sql, new { Word = word, Translation = translation, Stack =  stackId });

            return rowsAffected > 0;
        }
        catch (Exception)
        {
            return false;
        }
    }
    
    public bool CreateStacks(string title)
    {
        if (string.IsNullOrEmpty(title)) 
            return false;

        if (title.Length > 100) 
            return false;

        try
        {
            using var connection = new SqlConnection(ConfigString);

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

    public bool UpdateFlashCard(int cardId, string? word, string? translation)
    {
        using var connection = new SqlConnection(this.ConfigString);

        string query;
        object parameters;

        if (!string.IsNullOrEmpty(word) && !string.IsNullOrEmpty(translation))
        {
            query = "UPDATE FlashCards SET Word = @Word, Translation = @Translation WHERE Id = @Id";
            parameters = new { Word = word, Translation = translation, Id = cardId };
        }
        else if (!string.IsNullOrEmpty(word))
        {
            query = "UPDATE FlashCards SET Word = @Word WHERE Id = @Id";
            parameters = new { Word = word, Id = cardId };
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

    public bool DeleteFlashCard(int cardId)
    {
        try
        {
            using var connection = new SqlConnection(ConfigString);
            var rowsAffected = connection.Execute("DELETE FROM FlashCards WHERE Id = @Id", new { Id = cardId });
            return rowsAffected > 0;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool DeleteStack(int stackId)
    {
        try
        {
            using var connection = new SqlConnection(ConfigString);
            var rowsAffected = connection.Execute("DELETE FROM Stacks WHERE Id = @Id", new { id = stackId });
            return rowsAffected > 0;
        }
        catch { return false; }
    }
    public bool InsertStudySession(int stackId, int totalCards, int totalAttempts, TimeSpan duration)
    {
        try
        {
            using var connection = new SqlConnection(ConfigString);

            string sql = @"
            INSERT INTO StudySessions (StackId, SessionDate, TotalCards, TotalAttempts, CorrectAnswers, StudyDuration) 
            VALUES (@StackId, @SessionDate, @TotalCards, @TotalAttempts, @CorrectAnswers, @StudyDuration)";

            var rowsAffected = connection.Execute(sql, new
            {
                StackId = stackId,
                SessionDate = DateTime.Now,
                TotalCards = totalCards,
                TotalAttempts = totalAttempts,
                CorrectAnswers = totalCards,
                StudyDuration = duration
            });

            return rowsAffected > 0;
        }
        catch (Exception)
        {
            return false;
        }
    }
    public List<StudySessionDto?> GetStudySession(int? stackId = null)
    {
        try
        {
            using var connection = new SqlConnection(ConfigString);

            string sql;
            object parameters;

            if (stackId.HasValue)
            {
                sql = @"SELECT Id, StackId, SessionDate, TotalCards, TotalAttempts, CorrectAnswers, StudyDuration 
                    FROM StudySessions 
                    WHERE StackId = @StackId 
                    ORDER BY SessionDate DESC";
                parameters = new { StackId = stackId.Value };
            }
            else
            {
                sql = @"SELECT Id, StackId, SessionDate, TotalCards, TotalAttempts, CorrectAnswers, StudyDuration 
                    FROM StudySessions 
                    ORDER BY SessionDate DESC";
                parameters = new { };
            }

            var studySessions = connection.Query<StudySessionDto>(sql, parameters).ToList();

            return studySessions.Cast<StudySessionDto?>().ToList();
        }
        catch (Exception)
        {
            return new List<StudySessionDto?>();
        }
    }
}
 