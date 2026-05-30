using CODERAMA.Documents.API.Models;

namespace CODERAMA.Documents.API.Serialization;

public interface IDocumentSerializer
{
    string ContentType { get; }
    byte[] Serialize(Document document);
}
