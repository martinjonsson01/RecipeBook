namespace RecipeBook.Core.Domain
{
    public interface IShallowCloneable<out T>
    {
        public T ShallowClone();
    }
}