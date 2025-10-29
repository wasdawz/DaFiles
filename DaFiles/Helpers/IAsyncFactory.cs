using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace DaFiles.Helpers;

public interface IAsyncFactory<TKey, TValue>
{
    public Task<bool> TryGenerateItem(TKey key, [NotNullWhen(true)] out TValue? item);
}
