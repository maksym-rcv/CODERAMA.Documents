namespace CODERAMA.Documents.API.Serialization;

public interface IDocumentSerializerResolver
{
    IDocumentSerializer? Resolve(string? acceptHeader);
}
