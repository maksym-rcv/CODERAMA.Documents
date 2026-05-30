using CODERAMA.Documents.API.Models;
using CODERAMA.Documents.API.Serialization;
using CODERAMA.Documents.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CODERAMA.Documents.API.Controllers;

[ApiController]
[Route("documents")]
public sealed class DocumentsController : ControllerBase
{
    private readonly IDocumentService _documentService;
    private readonly IDocumentSerializerResolver _serializerResolver;

    public DocumentsController(IDocumentService documentService, IDocumentSerializerResolver serializerResolver)
    {
        _documentService = documentService;
        _serializerResolver = serializerResolver;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] DocumentRequest request, CancellationToken cancellationToken)
    {
        var result = await _documentService.CreateAsync(request, cancellationToken);

        return result switch
        {
            CreateDocumentResult.Created => CreatedAtAction(
                nameof(GetById),
                new { id = request.Id },
                null
            ),

            CreateDocumentResult.AlreadyExists => Conflict(new { message = $"Document with id '{request.Id}' already exists." }),
            _ => BadRequest(new { message = "Document must contain non-empty id, tags and data." })
        };
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] DocumentRequest request, CancellationToken cancellationToken)
    {
        var result = await _documentService.UpdateAsync(id, request, cancellationToken);

        return result switch
        {
            UpdateDocumentResult.Updated => NoContent(),
            UpdateDocumentResult.NotFound => NotFound(new { message = "Document with id '{id}' was not found." }),
            _ => BadRequest(new { message = "Request is invalid. Route id must match body id." })
        };
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
    {
        var document = await _documentService.GetByIdAsync(id, cancellationToken);

        if(document is null)
        {
            return NotFound(new { message = $"Document with id '{id}' was not found." });
        }

        var acceptHeader = Request.Headers.Accept.ToString();
        var serializer = _serializerResolver.Resolve(acceptHeader);

        if(serializer is null)
        {
            return StatusCode(StatusCodes.Status406NotAcceptable, new { meaasge = $"Requested format '{acceptHeader}' is not supported." });
        }

        var body = serializer.Serialize(document);

        return File(body, serializer.ContentType);
    }
}
