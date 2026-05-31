using CODERAMA.Documents.API.Serialization;
using FluentAssertions;

namespace CODERAMA.Documents.Tests.Serialization;

public sealed class DocumentSerializerResolverTests
{
    private readonly DocumentSerializerResolver _resolver = new(new IDocumentSerializer[]
    {
        new JsonDocumentSerializer(),
        new XmlDocumentSerializer(),
        new MessagePackDocumentSerializer()
    });

    [Fact]
    public void Resolve_ShouldReturnJsonSerializer_WhenAcceptHeaderIsJson()
    {
        var serializer = _resolver.Resolve("application/json");

        serializer.Should().BeOfType<JsonDocumentSerializer>();
    }

    [Fact]
    public void Resolve_ShouldReturnXmlSerializer_WhenAcceptHeaderIsXml()
    {
        var serializer = _resolver.Resolve("application/xml");

        serializer.Should().BeOfType<XmlDocumentSerializer>();
    }

    [Fact]
    public void Resolve_ShouldReturnMessagePackSerializer_WhenAcceptHeaderIsMessagePack()
    {
        var serializer = _resolver.Resolve("application/x-msgpack");

        serializer.Should().BeOfType<MessagePackDocumentSerializer>();
    }

    [Fact]
    public void Resolve_ShouldReturnJsonSerializer_WhenAcceptHeaderIsMissing()
    {
        var serializer = _resolver.Resolve(null);

        serializer.Should().BeOfType<JsonDocumentSerializer>();
    }

    [Fact]
    public void Resolve_ShouldReturnJsonSerializer_WhenAcceptHeaderAllowsAnyFormat()
    {
        var serializer = _resolver.Resolve("*/*");

        serializer.Should().BeOfType<JsonDocumentSerializer>();
    }

    [Fact]
    public void Resolve_ShouldReturnNull_WhenFormatIsNotSupported()
    {
        var serializer = _resolver.Resolve("text/csv");

        serializer.Should().BeNull();
    }
}
