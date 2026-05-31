using System.Text.Json;
using CODERAMA.Documents.API.Models;
using CODERAMA.Documents.API.Services;
using CODERAMA.Documents.API.Storage;
using FluentAssertions;

namespace CODERAMA.Documents.Tests.Services;

public sealed class DocumentServiceTests
{
    private readonly DocumentService _service = new(new InMemoryDocumentStorage());

    [Fact]
    public async Task CreateAsync_ShouldCreateDocument_WhenRequestIsValid()
    {
        var request = CreateRequest("document-1");
        var result = await _service.CreateAsync(request, CancellationToken.None);
        
        result.Should().Be(CreateDocumentResult.Created);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnAlreadyExists_WhenDocumentWithSameIdExists()
    {
        var request = CreateRequest("document-1");
        await _service.CreateAsync(request, CancellationToken.None);
        var result = await _service.CreateAsync(request, CancellationToken.None);
        
        result.Should().Be(CreateDocumentResult.AlreadyExists);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task CreateAsync_ShouldReturnInvalid_WhenIdIsEmpty(string id)
    {
        var request = CreateRequest(id);
        var result = await _service.CreateAsync(request, CancellationToken.None);

        result.Should().Be(CreateDocumentResult.Invalid);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnDocument_WhenDocumentExists()
    {
        var request = CreateRequest("document-1");
        await _service.CreateAsync(request, CancellationToken.None);
        var document = await _service.GetByIdAsync("document-1", CancellationToken.None);

        document.Should().NotBeNull();
        document!.Id.Should().Be("document-1");
        document.Tags.Should().Contain(".net");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenDocumentDoesNotExist()
    {
        var document = await _service.GetByIdAsync("missing-document", CancellationToken.None);

        document.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateDocument_WhenDocumentExists()
    {
        var originalRequest = CreateRequest("document-1");
        var updatedRequest = CreateRequest("document-1", "updated-value");

        await _service.CreateAsync(originalRequest, CancellationToken.None);

        var result = await _service.UpdateAsync("document-1", updatedRequest, CancellationToken.None);
        var updatedDocument = await _service.GetByIdAsync("document-1", CancellationToken.None);

        result.Should().Be(UpdateDocumentResult.Updated);
        updatedDocument.Should().NotBeNull();
        updatedDocument!.Data.GetProperty("some").GetString().Should().Be("updated-value");
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNotFound_WhenDocumentDoesNotExist()
    {
        var request = CreateRequest("document-1");
        var result = await _service.UpdateAsync("document-1", request, CancellationToken.None);

        result.Should().Be(UpdateDocumentResult.NotFound);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnInvalid_WhenRouteIdDoesNotMatchBodyId()
    {
        var request = CreateRequest("document-1");
        var result = await _service.UpdateAsync("different-id", request, CancellationToken.None);

        result.Should().Be(UpdateDocumentResult.Invalid);
    }

    private static DocumentRequest CreateRequest(string id, string value = "data")
    {
        var json = $"{{ \"some\": \"{value}\", \"optional\": \"fields\" }}";

        using var jsonDocument = JsonDocument.Parse(json);

        return new DocumentRequest
        {
            Id = id,
            Tags = ["important", ".net"],
            Data = jsonDocument.RootElement.Clone()
        };
    }
}
