using System.Security;

namespace DaFiles.Models;

public record WebDavRepositoryConfig(string HostUrl, string? Username, SecureString? Password, string? RootPath);
