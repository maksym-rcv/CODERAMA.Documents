using System.Text.Json;
using CODERAMA.Documents.API.Models;
using CODERAMA.Documents.API.Storage;
using FluentAssertions;

namespace CODERAMA.Documents.Tests.Storage;

public sealed class InMemoryDocumentStorageTests
{
    private readonly InMemoryDocumentStorage _storage = new();


    [Fact]
    public async Task CreateAsync_ShouldStoreDocument()
    {
        var document = CreateDocument("document-1");

        var created = await _storage.CreateAsync(document, CancellationToken.None);
        var storedDocument = await _storage.GetByIdAsync("document-1", CancellationToken.None);

        created.Should().BeTrue();
        storedDocument.Should().NotBeNull();
        storedDocument!.Id.Should().Be("document-1");
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnFalse_WhenDocumentAlreadyExists()
    {
        var document = CreateDocument("document-1");

        await _storage.CreateAsync(document, CancellationToken.None);

        var createdAgain = await _storage.CreateAsync(document, CancellationToken.None);

        createdAgain.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnTrue_WhenDocumentExists()
    {
        var document = CreateDocument("document-1");
        var updatedDocument = CreateDocument("document-1", "updated-value");

        await _storage.CreateAsync(document, CancellationToken.None);

        var updated = await _storage.UpdateAsync(updatedDocument, CancellationToken.None);
        var storedDocument = await _storage.GetByIdAsync("document-1", CancellationToken.None);

        updated.Should().BeTrue();
        storedDocument!.Data.GetProperty("some").GetString().Should().Be("updated-value");
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnFalse_WhenDocumentDoesNotExist()
    {
        var document = CreateDocument("document-1");

        var updated = await _storage.UpdateAsync(document, CancellationToken.None);

        updated.Should().BeFalse();
    }

    private static Document CreateDocument(string id, string value = "data")
    {
        using var jsonDocument = JsonDocument.Parse($$"""
        {
            "some": "{{value}}"
        }
        """);

        return new Document
        {
            Id = id,
            Tags = ["important", ".net"],
            Data = jsonDocument.RootElement.Clone()
        };
    }
}
