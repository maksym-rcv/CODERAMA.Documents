using System.Text.Json;

namespace CODERAMA.Documents.API.Models;

public sealed class Document
{
    public required string Id { get; init; }

    public required IReadOnlyCollection<string> Tags { get; init; }

    public required JsonElement Data { get; init; }
}
