using System.Text.Json;
using CODERAMA.Documents.API.Models;
using Document = CODERAMA.Documents.API.Models.Document;

namespace CODERAMA.Documents.API.Serialization;

public sealed class JsonDocumentSerializer : IDocumentSerializer
{
    public string ContentType => "application/json";
    public byte[] Serialize(Document document)
    {
        ArgumentNullException.ThrowIfNull(document);
        return JsonSerializer.SerializeToUtf8Bytes(document, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        });
    }
}
