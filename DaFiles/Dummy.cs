using DaFiles.Helpers;
using DaFiles.Models;
using DaFiles.Services;
using DaFiles.ViewModels;
using DynamicData;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DaFiles;

internal static class Dummy
{
    internal class DataStore<T>(int operationDelayMilliseconds = 0) : IDataStore<T>
    {
        public int OperationDelayMilliseconds { get; set; } = operationDelayMilliseconds;

        private readonly List<T> _items = [];

        public async Task DeleteAsync(T item)
        {
            if (OperationDelayMilliseconds > 0)
                await Task.Delay(OperationDelayMilliseconds);
            _items.Remove(item);
        }

        public async Task SaveAsync(IEnumerable<T> items)
        {
            if (OperationDelayMilliseconds > 0)
                await Task.Delay(OperationDelayMilliseconds);
            _items.AddRange(items);
        }
    }

    private static readonly Dictionary<Type, object> _dataStores = [];

    public static RepositoriesSettingsViewModel CreateDummyRepositoriesSettingsViewModel()
    {
        SourceList<Repository> repositories = new();
        repositories.AddRange(CreateDummyRepositories(4));
        return new RepositoriesSettingsViewModel(repositories, GetDummyDataStore<Repository>());
    }

    public static DataStore<T> GetDummyDataStore<T>()
    {
        DataStore<T> store = (DataStore<T>)_dataStores.GetOrAdd(typeof(T), () => new DataStore<T>());
        return store;
    }

    public static IEnumerable<Repository> CreateDummyRepositories(int count)
    {
        for (int i = 0; i < count; i++)
        {
            yield return new Repository(
                new DummyRepositoryConfig(Guid.NewGuid().ToString(), $"Directory {i + 1}", null));
        }
    }
}
