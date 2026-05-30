using CODERAMA.Documents.API.Models;
using CODERAMA.Documents.API.Storage;

namespace CODERAMA.Documents.API.Services;

public sealed class DocumentService : IDocumentService
{
    private readonly IDocumentStorage _storage;
    public DocumentService(IDocumentStorage storage)
    {
        _storage = storage;
    }

    public async Task<CreateDocumentResult> CreateAsync(DocumentRequest request, CancellationToken cancellationToken)
    {
        if (!IsValid(request))
        {
            return CreateDocumentResult.Invalid;
        }
        var document = new Document
        {
            Id = request.Id.Trim(),
            Tags = request.Tags,
            Data = request.Data
        };

        var created = await _storage.CreateAsync(document, cancellationToken);

        return created ? CreateDocumentResult.Created : CreateDocumentResult.AlreadyExists;
    }

    public Task<Document?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return Task.FromResult<Document?>(null);
        }

        return _storage.GetByIdAsync(id.Trim(), cancellationToken);
    }


    public async Task<UpdateDocumentResult> UpdateAsync(string id, DocumentRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(id) || !IsValid(request))
        {
            return UpdateDocumentResult.Invalid;
        }

        if (!string.Equals(id.Trim(), request.Id.Trim(), StringComparison.Ordinal))
        {
            return UpdateDocumentResult.Invalid;
        }

        var document = new Document
        {
            Id = request.Id.Trim(),
            Tags = request.Tags,
            Data = request.Data
        };

        var updated = await _storage.UpdateAsync(document, cancellationToken);

        return updated
            ? UpdateDocumentResult.Updated
            : UpdateDocumentResult.NotFound;
    }

    private static bool IsValid(DocumentRequest request)
    {
        return !string.IsNullOrWhiteSpace(request.Id) && request.Tags is not null
            && request.Tags.Count > 0
            && request.Data.ValueKind is not System.Text.Json.JsonValueKind.Undefined;
    }
}
