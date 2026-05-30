using CODERAMA.Documents.API.Models;

namespace CODERAMA.Documents.API.Storage;

public interface IDocumentStorage
{
    Task<bool> CreateAsync(Document document, CancellationToken cancellationToken);

    Task<Document?> GetByIdAsync(string id, CancellationToken cancellationToken);

    Task<bool> UpdateAsync(Document document, CancellationToken cancellationToken);
}
