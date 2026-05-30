using System.Text.Json;
using CODERAMA.Documents.API.Models;
using MessagePack;

namespace CODERAMA.Documents.API.Serialization;

public sealed class MessagePackDocumentSerializer : IDocumentSerializer
{
    public string ContentType => "application/x-msgpack";
    public byte[] Serialize(Document document)
    {
        ArgumentNullException.ThrowIfNull(document);

        var dto = new MessagePackDocumentDto
        {
            Id = document.Id,
            Tags = document.Tags.ToArray(),
            Data = JsonSerializer.Deserialize<object>(document.Data.GetRawText())
        };

        return MessagePackSerializer.Serialize(dto);
    }
    [MessagePackObject]
    public sealed class MessagePackDocumentDto
    {
        [Key(0)]
        public required string Id { get; init; }

        [Key(1)]
        public required string[] Tags { get; init; }

        [Key(2)]
        public object? Data { get; init; }
    }
}
