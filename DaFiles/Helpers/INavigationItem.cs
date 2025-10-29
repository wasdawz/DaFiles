namespace DaFiles.Helpers;

public interface INavigationItem<TKey>
{
    public TKey NavigationKey { get; }
}
