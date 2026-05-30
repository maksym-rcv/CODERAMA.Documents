using CODERAMA.Documents.API.Models;

namespace CODERAMA.Documents.API.Services;

public interface IDocumentService
{
    Task<CreateDocumentResult> CreateAsync(DocumentRequest request, CancellationToken cancellationToken);
    Task<Document?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<UpdateDocumentResult> UpdateAsync(string id, DocumentRequest request, CancellationToken cancellationToken);
}
