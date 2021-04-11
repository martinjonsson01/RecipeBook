using System.Data;

using Dapper;

using Newtonsoft.Json;

using RecipeBook.Core.Domain.Units;

namespace RecipeBook.Infrastructure.Persistence
{
    public class UnitTypeHandler : SqlMapper.TypeHandler<Unit>
    {
        public override void SetValue(IDbDataParameter parameter, Unit value)
        {
            parameter.Value = JsonConvert.SerializeObject(value);
            parameter.DbType = DbType.String;
        }

        public override Unit Parse(object value)
        {
            return JsonConvert.DeserializeObject<Unit>(value.ToString() ?? string.Empty);
        }
    }
}