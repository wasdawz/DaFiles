using System.Collections.Generic;
using System.Threading.Tasks;

namespace DaFiles.Services;

public interface IDataStore<T>
{
    Task SaveAsync(IEnumerable<T> items);
}
