# Flashcards Console Application

A comprehensive console-based flashcards application built with C# and .NET 8, featuring an interactive study system with progress tracking and beautiful terminal UI.

## Features

- **Deck Management**: Create, view, and delete flashcard decks
- **Card Management**: Add, edit, and delete individual flashcards
- **Interactive Study Sessions**: Practice with randomized flashcards
- **Study History**: Track your learning progress over time
- **Persistent Storage**: SQL Server database with automatic initialization

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (LocalDB is sufficient)

### Installation

1. **Clone the repository**
   ```bash
   git clone <your-repository-url>
   cd CodeReviews.Console.Flashcards.Gincaza
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Build the solution**
   ```bash
   dotnet build
   ```

4. **Run the application**
   ```bash
   cd FlashcardsApp
   dotnet run
   ```

The application will automatically create the tables on first run. But not the database.

## Project Structure

```
CodeReviews.Console.Flashcards.Gincaza/
   Business Logic/           # Business logic and DTOs
      DTO/                   # Data transfer objects
      Interfaces/            # Business interfaces
      Messages/              # Operation result models
      BusinessLogic.cs       # Core business logic
   DataAccess/              # Data access layer
      DataAccessRepository.cs # Database operations
   FlashcardsApp/           # Console application entry point
      Program.cs             # Application startup
   PresentationLayer/       # User interface layer
      Helpers/               # UI utility classes
      Managers/              # Feature managers
      FlashcardsUI.cs        # Main UI coordinator
```

## ?? How to Use

### 1. Creating Your First Deck
- Select "Create deck" from the main menu
- Enter a unique deck name
- Your deck is ready for flashcards!

### 2. Adding Flashcards
- Choose "Manage Deck" ? Select your deck
- Select "Create Card"
- Enter the word and its translation
- Repeat to build your deck

### 3. Studying
- Select "Start Study Lesson"
- Choose a deck to study
- Type translations for each word
- Cards you get wrong will repeat until mastered
- View your performance summary at the end

### 4. Tracking Progress
- Select "View Study History"
- See all your study sessions or filter by deck
- Monitor your learning progress over time

## Technologies Used

- **Framework**: .NET 8
- **Language**: C# 12
- **Database**: SQL Server with Dapper ORM
- **UI**: Spectre.Console for rich terminal interface
- **Architecture**: Clean Architecture with separation of concerns

## Database Schema

The application uses three main tables:

- **Stacks**: Stores flashcard decks
- **FlashCards**: Stores individual flashcards with word/translation pairs
- **StudySessions**: Tracks study session history and performance

## Configuration

The application connects to SQL Server using integrated security:
```
Data Source=localhost;Initial Catalog=FlashcardsDB;Integrated Security=true;TrustServerCertificate=true;
```

To modify the connection string, edit the `configString` property in `DataAccessRepository.cs`.

## Key Features Explained

### Smart Study System
- Cards are shuffled for each session
- Incorrect answers cause cards to be re-queued
- Sessions continue until all cards are mastered
- Progress tracking with attempt counting

### Interactive Console UI
- Real-time progress indicators during study
- Clear feedback for correct/incorrect answers
- Formatted duration and statistics display

### Data Persistence
- Automatic database and table creation
- Foreign key relationships for data integrity
- Study session history with detailed metrics
- Robust error handling and validation

## License

This project is part of the C# Academy coding challenges. Feel free to use it for learning purposes.

## Acknowledgments

- [The C# Academy](https://thecsharpacademy.com/) for the project inspiration
- [Spectre.Console](https://spectreconsole.net/) for the beautiful terminal UI
- [Dapper](https://github.com/DapperLib/Dapper) for the lightweight ORM

---

*Happy studying!*
