namespace CODERAMA.Documents.API.Serialization;

public sealed class DocumentSerializerResolver : IDocumentSerializerResolver
{
    private readonly IReadOnlyCollection<IDocumentSerializer> _serializers;

    public DocumentSerializerResolver(IEnumerable<IDocumentSerializer> serializers)
    {
        _serializers = serializers.ToArray();
    }

    public IDocumentSerializer? Resolve(string? acceptHeader)
    {
        if (string.IsNullOrWhiteSpace(acceptHeader) || acceptHeader.Contains("*/*"))
        {
            return _serializers.FirstOrDefault(serializer =>
                serializer.ContentType == "application/json");
        }

        return _serializers.FirstOrDefault(serializer =>
            acceptHeader.Contains(serializer.ContentType, StringComparison.OrdinalIgnoreCase));
    }
}
