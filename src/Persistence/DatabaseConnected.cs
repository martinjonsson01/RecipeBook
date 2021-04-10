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
            if (connectionString == "")
            {
                ConnectionString = Environment.GetEnvironmentVariable("RecipeBookDBConnectionString") ?? throw
                    new Exception("Could not locate RecipeBookDBConnectionString environment variable.");
            }
            ConnectionString = connectionString;
        }
    }
}