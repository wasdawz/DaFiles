using System.Security;

namespace DaFiles.Services;

public interface ISecretStore
{
    SecureString? Read(string key, SecureString? target = null);

    void Write(string key, SecureString secret);

    bool Delete(string key);
}
