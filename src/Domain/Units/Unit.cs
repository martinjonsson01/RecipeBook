namespace RecipeBook.Core.Domain.Units
{
    public record Unit(double Value)
    {
        public int Id { get; set; }

        public static readonly Unit Zero = new(0);
    }
}