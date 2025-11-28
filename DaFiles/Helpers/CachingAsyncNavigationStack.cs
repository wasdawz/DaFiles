using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DaFiles.Helpers;

/// <summary>
/// </summary>
/// <typeparam name="TKey">Type used for retrieving of cached navigation items.</typeparam>
/// <typeparam name="TItem">Type of cached navigation items.</typeparam>
public partial class CachingAsyncNavigationStack<TKey, TItem>(Func<TKey, Task<TItem?>> asyncFactory) : ObservableObject where TItem : INavigationItem<TKey> where TKey : notnull
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(GoBackCommand))]
    [NotifyCanExecuteChangedFor(nameof(GoForwardCommand))]
    private int navigationStackIndex = -1;

    public bool CanGoBack => NavigationStackIndex > 0;
    public bool CanGoForward => NavigationStackIndex < _navigationStack.Count - 1;
    public TItem? CurrentItem => NavigationStackIndex < 0 ? default : _navigationStack[NavigationStackIndex];

    public TimeSpan CacheItemRefreshMaxDelay { get; set; } = TimeSpan.FromMilliseconds(5);

    public event Action? CurrentItemChanged;
    public event Action<LoadingCachedItemFailedEventArgs<TItem>>? LoadingCachedItemFailed;

    private readonly Func<TKey, Task<TItem?>> _asyncFactory = asyncFactory;
    private readonly Dictionary<TKey, TItem> _cache = [];
    private readonly List<TItem> _navigationStack = [];

    [RelayCommand]
    public async Task SetTopItemAsync(TKey itemKey)
    {
        if (CurrentItem is TItem currentItem && Equals(currentItem.NavigationKey, itemKey))
            return;

        // Clear navigation stack above current item.
        _navigationStack.RemoveRange(NavigationStackIndex + 1, _navigationStack.Count - NavigationStackIndex - 1);

        if (_cache.TryGetValue(itemKey, out var item))
        {
            if (!await TryLoadCachedItemAsync(item))
                return;
        }
        else
        {
            // Item not in cache - create.
            item = await _asyncFactory(itemKey);

            if (item is null)
                return;

            _cache.TryAdd(itemKey, item);
        }

        _navigationStack.Add(item);
        NavigationStackIndex++;
        OnCurrentItemChanged();
    }

    [RelayCommand(CanExecute = nameof(CanGoBack))]
    public async Task GoBackAsync()
    {
        if (NavigationStackIndex <= 0)
            return;

        int newIndex = NavigationStackIndex - 1;

        if (!await TryLoadCachedItemFromStackAsync(newIndex))
            return;

        NavigationStackIndex = newIndex;
        OnCurrentItemChanged();
    }

    [RelayCommand(CanExecute = nameof(CanGoForward))]
    public async Task GoForwardAsync()
    {
        if (_navigationStack.Count - 1 == NavigationStackIndex)
            return;

        int newIndex = NavigationStackIndex + 1;

        if (!await TryLoadCachedItemFromStackAsync(newIndex))
            return;

        NavigationStackIndex = newIndex;
        OnCurrentItemChanged();
    }

    private async Task<bool> TryLoadCachedItemFromStackAsync(int navigationStackIndex)
    {
        return await TryLoadCachedItemAsync(_navigationStack[navigationStackIndex]);
    }

    private async Task<bool> TryLoadCachedItemAsync(TItem item)
    {
        if (item is IRefreshAsync refreshableItem)
        {
            var refreshTask = refreshableItem.RefreshAsync();
            var timeoutTask = Task.Delay(CacheItemRefreshMaxDelay);

            if (await Task.WhenAny(refreshTask, timeoutTask) == timeoutTask)
            {
                _ = HandleRefreshAsync();
                return true;
            }

            return await HandleRefreshAsync();

            async Task<bool> HandleRefreshAsync()
            {
                try
                {
                    await refreshTask;
                    return true;
                }
                catch (Exception ex)
                {
                    LoadingCachedItemFailed?.Invoke(new()
                    {
                        Item = item,
                        Error = ex
                    });
                    return false;
                }
            }
        }
        return true;
    }

    private void OnCurrentItemChanged()
    {
        OnPropertyChanged(nameof(CurrentItem));
        CurrentItemChanged?.Invoke();
    }
}

public class LoadingCachedItemFailedEventArgs<T>
{
    public required T Item { get; init; }

    public Exception? Error { get; init; }
}
