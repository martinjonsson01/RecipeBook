namespace RecipeBook.Core.Domain.Recipes
{
    public class Step : BaseEntity
    {
        public int    Number      { get; set; } = 0;
        public string Instruction { get; set; } = string.Empty;
    }
}