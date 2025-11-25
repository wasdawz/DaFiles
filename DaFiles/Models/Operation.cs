using System.Collections.Generic;

namespace DaFiles.Models;

public enum TransferOperationType
{
    Copy,
    Cut,
}

public class Operation
{
}

public class TransferOperation : Operation
{
    public required TransferOperationType OperationType { get; init; }

    public required Directory Source { get; init; }

    public Directory? Destination { get; set; }

    public required IList<DirectoryItem> Items { get; init; }
}

public class DeleteOperation : Operation
{
    public required Directory ParentDirectory { get; init; }

    public required IList<DirectoryItem> Items { get; init; }

    public required bool Permanent { get; init; }
}
