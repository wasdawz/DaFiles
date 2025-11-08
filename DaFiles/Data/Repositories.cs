using Avalonia.Controls;
using Avalonia.Platform.Storage;
using DaFiles.Helpers;
using DaFiles.Models;
using DaFiles.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text.Json;
using System.Threading.Tasks;

namespace DaFiles.Data;

public class Repositories(string storagePath, Func<TopLevel?> topLevelGetter, ISecretStore secretStore) : IDataStore<Repository>
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        Converters = { new SecretJsonConverter() }
    };

    private readonly string _storagePath = storagePath;
    private readonly Func<TopLevel?> _topLevelGetter = topLevelGetter;
    private readonly ISecretStore _secretStore = secretStore;

    public async Task SaveAsync(IEnumerable<Repository> repositories)
    {
        if (_topLevelGetter()?.StorageProvider is not IStorageProvider storageProvider ||
            await storageProvider.TryGetFolderFromPathAsync(_storagePath) is not IStorageFolder folder)
            return;

        foreach (RepositoryConfig config in repositories.Select(r => r.Config).Where(c => c.IsSaveable))
        {
            if (await folder.CreateFileAsync($"{config.Id}.json") is not IStorageFile file)
                continue;

            if (config.Password is SecureString password)
            {
                _secretStore.Write(config.Id, password);
            }

            using Stream fileStream = await file.OpenWriteAsync();
            await JsonSerializer.SerializeAsync(fileStream, config, SerializerOptions);
        }
    }

    public async Task<List<Repository>?> ReadAllAsync()
    {
        if (_topLevelGetter()?.StorageProvider is not IStorageProvider storageProvider ||
            await storageProvider.TryGetFolderFromPathAsync(_storagePath) is not IStorageFolder folder)
            return null;

        List<Repository> items = [];

        await foreach (IStorageItem item in folder.GetItemsAsync())
        {
            if (item is not IStorageFile file)
                continue;

            using Stream fileStream = await file.OpenReadAsync();

            if (await JsonSerializer.DeserializeAsync<RepositoryConfig>(fileStream, SerializerOptions) is not RepositoryConfig config)
                continue;

            if (config.Password is SecureString password)
            {
                _secretStore.Read(config.Id, password);
            }

            Repository repository = new(config);
            items.Add(repository);
        }

        return items;
    }
}
