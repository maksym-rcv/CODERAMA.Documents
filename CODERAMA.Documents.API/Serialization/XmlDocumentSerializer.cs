using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using CODERAMA.Documents.API.Models;

namespace CODERAMA.Documents.API.Serialization;

public sealed class XmlDocumentSerializer : IDocumentSerializer
{
    public string ContentType => "application/xml";
    public byte[] Serialize(Document document)
    {
        ArgumentNullException.ThrowIfNull(document);
        var root = new XElement("document",
            new XElement("Id", document.Id),
            new XElement("tags", document.Tags.Select(tag => new XElement("tag", tag))),
            new XElement("data", ConvertJsonElementToXml(document.Data))
        );

        var xml = new XDocument(root).ToString(SaveOptions.DisableFormatting);

        return Encoding.UTF8.GetBytes(xml.ToString());
    }

    private static object ConvertJsonElementToXml(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.Object => element.EnumerateObject()
                .Select(property => new XElement(property.Name, ConvertJsonElementToXml(property.Value))),

            JsonValueKind.Array => element.EnumerateArray()
                .Select(item => new XElement("item", ConvertJsonElementToXml(item))),

            JsonValueKind.String => element.GetString() ?? string.Empty,

            JsonValueKind.Number => element.GetRawText(),

            JsonValueKind.True => true,

            JsonValueKind.False => false,

            JsonValueKind.Null => string.Empty,

            _ => element.GetRawText()
        };
    }
}
