using Business_Logic;
using DataAccess;
using PresentationLayer;

// Dependency injection
var dataAccess = new DataAccessRepository();
var businessLogic = new BusinessLogic(dataAccess);

// Create and execute the app
var app = new FlashcardsUI(businessLogic);
await app.RunAsync();
