namespace RecipeBook.Presentation.WebApp.Client.Shared
{
    public interface IHasFocusedItem<TItem>
    {
        TItem? FocusedItem { get; set; }
    }
}