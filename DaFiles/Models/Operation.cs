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
