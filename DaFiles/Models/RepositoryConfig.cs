using Avalonia.Controls;
using DaFiles.Services.Repositories;
using System;
using System.Security;
using System.Text.Json.Serialization;

namespace DaFiles.Models;

[JsonDerivedType(typeof(WebDavRepositoryConfig), typeDiscriminator: "WebDAV")]
public abstract record RepositoryConfig(string Id, string Name, SecureString? Password)
{
    public abstract bool IsSaveable { get; }

    public abstract IRepository CreateService();
}

public record LocalRepositoryConfig(string Name, Func<TopLevel?> TopLevelGetter) : RepositoryConfig(string.Empty, Name, null)
{
    public override bool IsSaveable => false;

    public override IRepository CreateService() => new LocalRepository(TopLevelGetter);
}

public record WebDavRepositoryConfig(string Id, string Name, string HostUrl, string? Username, SecureString? Password, string? RootPath) : RepositoryConfig(Id, Name, Password)
{
    public override bool IsSaveable => true;

    public override IRepository CreateService() => new WebDavRepository(this);
}
