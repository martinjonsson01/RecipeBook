using System;

using Microsoft.Extensions.Logging;

namespace RecipeBook.Infrastructure.Persistence
{
    public abstract class DatabaseConnected<T>
    {
        protected readonly string     ConnectionString;
        protected readonly ILogger<T> Logger;

        protected DatabaseConnected(ILogger<T> logger, string connectionString = "")
        {
            Logger = logger;
            if (string.IsNullOrEmpty(connectionString))
            {
                Logger.LogInformation(
                    "No connection string provided, using one from environment variable 'RecipeBookDBConnectionString'");
                ConnectionString = Environment.GetEnvironmentVariable("RecipeBookDBConnectionString") ?? throw
                    new Exception("Could not locate RecipeBookDBConnectionString environment variable.");
                Logger.LogInformation("Using connection string: {ConnectionString}", ConnectionString);
            }
            else
            {
                Logger.LogInformation("Connection string provided: {ConnectionString}", connectionString);
                ConnectionString = connectionString;
            }
        }
    }
}