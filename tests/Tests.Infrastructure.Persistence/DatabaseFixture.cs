using System;

using Npgsql;

using Respawn;

namespace Tests.Infrastructure.Persistence
{
    public class DatabaseFixture : IDisposable
    {
        public DatabaseFixture()
        {
            ConnectionString = Environment.GetEnvironmentVariable("RecipeBookDB_TESTConnectionString") ?? throw
                new Exception("Could not locate RecipeBookDB_TESTConnectionString environment variable.");
            Db = new NpgsqlConnection(ConnectionString);

            Db.Open();

            Checkpoint.Reset(Db).Wait();
        }

        public string           ConnectionString { get; }
        public NpgsqlConnection Db               { get; }

        public void Dispose()
        {
            Db.Close();
            Db.Dispose();
            GC.SuppressFinalize(this);
        }

        public static readonly Checkpoint Checkpoint = new()
        {
            SchemasToInclude = new[]
            {
                "public"
            },
            DbAdapter = DbAdapter.Postgres
        };
    }
}