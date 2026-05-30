using System.Collections.Concurrent;
using CODERAMA.Documents.API.Models;

namespace CODERAMA.Documents.API.Storage;

public sealed class InMemoryDocumentStorage : IDocumentStorage
{
    private readonly ConcurrentDictionary<string, Document> _documents = new();

    public Task<bool> CreateAsync(Document document, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(document);
        var created = _documents.TryAdd(document.Id, document);
        return Task.FromResult(created);
    }

    public Task<Document?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        _documents.TryGetValue(id, out var document);
        return Task.FromResult(document);
    }

    public Task<bool> UpdateAsync(Document document, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(document);
        if (!_documents.ContainsKey(document.Id))
        {
            return Task.FromResult(false);
        }
        _documents[document.Id] = document;
        return Task.FromResult(true);
    }
}
