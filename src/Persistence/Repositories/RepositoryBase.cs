using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Dapper;

using Microsoft.Extensions.Logging;

using Npgsql;

using RecipeBook.Core.Application.Repository;

namespace RecipeBook.Infrastructure.Persistence.Repositories
{
    public abstract class RepositoryBase<TSelf, TResource, TKey>
        : DatabaseConnected<TSelf>, IResourcesRepository<TResource, TKey>
        where TResource : class
        where TSelf : class
    {
        protected RepositoryBase(
            ILogger<TSelf> logger,
            string?        tableName        = null,
            string         keyColumn        = "id",
            string         keyPropertyName  = "Id",
            string         connectionString = "")
            : base(logger, connectionString)
        {
            _table = tableName ?? $"{typeof(TResource).Name.ToLowerInvariant()}s";
            _keyColumn = keyColumn;
            _keyPropertyName = keyPropertyName;
            ExcludedPropertyNames = new List<string> { _keyPropertyName };
            LoadEntityProperties();
        }

        private readonly   string              _table;
        private readonly   string              _keyColumn;
        private readonly   string              _keyPropertyName;
        private            string              _columnNames;
        private            string              _propertyNames;
        private            string              _setColumnsToPropertyNames;
        protected readonly IList<string>       ExcludedPropertyNames;
        private            IEnumerable<string> _entityProperties;


        protected virtual string GetAllSql => $@"
                    SELECT * 
                      FROM {_table}
                     WHERE recipeid = :recipeId; 
            ";

        protected virtual string GetSql => $@"
                    SELECT * 
                      FROM {_table}
                     WHERE {_keyColumn} = :key
                       AND recipeid = :recipeId;
            ";

        protected virtual string ExistsSql => $@"
                    SELECT EXISTS(
                        SELECT *
                          FROM {_table}
                         WHERE {_keyColumn} = :key
                           AND recipeid = :recipeId
                    ); 
            ";

        protected virtual string CreateOrUpdateSql(string idQuery, int recipeId, TResource resource) => $@"
                    INSERT
                      INTO {_table}
                           ({_keyColumn}, recipeid{_columnNames})
                    VALUES ({idQuery}, {recipeId}{_propertyNames})
               ON CONFLICT ({_keyColumn}) 
                 DO UPDATE
                       SET recipeid = {recipeId}, {_setColumnsToPropertyNames}
                     WHERE {_table}.{_keyColumn} = :{_keyPropertyName}
                 RETURNING {_keyColumn};
            ";

        protected virtual string DeleteSql => $@"
                DELETE 
                  FROM {_table}
                 WHERE {_keyColumn} = :key
                   AND recipeid = :recipeId;
            ";

        protected virtual bool EntityKeyIsNull(dynamic entity) => entity.Id is null;

        protected virtual void AddTypeHandlers()
        {
        }

        public virtual async Task<IEnumerable<TResource>> GetAllAsync(string recipeName)
        {
            await using var db = new NpgsqlConnection(ConnectionString);

            int recipeId = await GetRecipeId(recipeName, db);

            return await db.QueryAsync<TResource>(GetAllSql, new { recipeId });
        }

        public virtual async Task<TResource?> GetAsync(string recipeName, TKey key)
        {
            await using var db = new NpgsqlConnection(ConnectionString);

            int recipeId = await GetRecipeId(recipeName, db);

            return await db.QuerySingleOrDefaultAsync<TResource>(GetSql, new { key, recipeId });
        }

        public virtual async Task<bool> ExistsAsync(string recipeName, TKey key)
        {
            await using var db = new NpgsqlConnection(ConnectionString);

            int recipeId = await GetRecipeId(recipeName, db);

            return await db.QuerySingleAsync<bool>(ExistsSql, new { key, recipeId });
        }

        public virtual async Task<TResource?> CreateOrUpdateAsync(string recipeName, TResource entity)
        {
            await using var db = new NpgsqlConnection(ConnectionString);

            int recipeId = await GetRecipeId(recipeName, db);

            try
            {
                string idQuery = EntityKeyIsNull(entity) ? "default" : $":{_keyPropertyName}";

                AddTypeHandlers();

                var insertedKey = await db.QuerySingleAsync<TKey>(
                    CreateOrUpdateSql(idQuery, recipeId, entity),
                    entity);

                SetEntityKey(entity, insertedKey);

                return entity;
            }
            catch (NpgsqlException e)
            {
                if (e.SqlState != "23505") throw;

                Logger.LogWarning("Could not create or update resource because of column conflict");
                return null;
            }
        }

        public virtual async Task DeleteAsync(string recipeName, TKey key)
        {
            await using var db = new NpgsqlConnection(ConnectionString);

            int recipeId = await GetRecipeId(recipeName, db);

            await db.ExecuteAsync(DeleteSql, new { key, recipeId });
        }

        protected virtual void SetEntityKey(dynamic entity, TKey key) => entity.Id = key!;

        private static Task<int> GetRecipeId(string recipeName, IDbConnection db)
        {
            return db.QuerySingleOrDefaultAsync<int>(@"
                SELECT id
                  FROM recipes
                 WHERE name = :recipeName
            ", new { recipeName });
        }
        
        protected void LoadEntityProperties()
        {
            _entityProperties = GetEntityPropertyNames();
            _columnNames = GetColumnNamesString();
            _propertyNames = GetPropertyNamesString();
            _setColumnsToPropertyNames = GetColumnsToPropertyNamesString();
        }
        
        private IEnumerable<string> GetEntityPropertyNames()
        {
            PropertyInfo[] propertyInfos = typeof(TResource).GetProperties();
            return propertyInfos
                   .Where(propertyInfo => !ExcludedPropertyNames.Contains(propertyInfo.Name))
                   .Select(propertyInfo => propertyInfo.Name);
        }

        /// <summary>
        /// Presents properties in following way:
        /// ", name, rating, imagepath"
        /// </summary>
        private string GetColumnNamesString()
        {
            StringBuilder sb = new();
            foreach (string propertyName in _entityProperties)
            {
                sb.Append(", ");
                sb.Append(propertyName.ToLowerInvariant());
            }
            return sb.ToString();
        }

        /// <summary>
        /// Presents properties in following way:
        /// ", :Name, :Rating, :ImagePath"
        /// </summary>
        private string GetPropertyNamesString()
        {
            StringBuilder sb = new();
            foreach (string propertyName in _entityProperties)
            {
                sb.Append(", :");
                sb.Append(propertyName);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Presents properties in following way:
        /// "name = :Name, rating = :Rating, imagepath = :ImagePath"
        /// </summary>
        private string GetColumnsToPropertyNamesString()
        {
            StringBuilder sb = new();
            foreach (string propertyName in _entityProperties)
            {
                if (!_entityProperties.First().Equals(propertyName))
                {
                    sb.Append(", ");
                }
                sb.Append(propertyName.ToLowerInvariant());
                sb.Append(" = :");
                sb.Append(propertyName);
            }
            return sb.ToString();
        }
    }
}