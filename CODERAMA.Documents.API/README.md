# CODERAMA Documents API

ASP.NET Core Web API service for storing and retrieving documents in multiple formats.

## Assignment

The service provides an API for storing documents sent as JSON payloads and retrieving them in different formats such as JSON, XML and MessagePack.

## Features

- Create document using POST
- Update document using PUT
- Retrieve document using GET
- Support for JSON, XML and MessagePack response formats
- Extensible serialization strategy
- Replaceable storage abstraction
- In-memory storage implementation
- Unit tests for service, storage and serializer resolver

## Tech Stack

- .NET 8
- ASP.NET Core Web API
- C#
- MessagePack
- xUnit
- FluentAssertions

## API Endpoints

### Create document

```http
POST /documents
Content-Type: application/json
```

Example request:

```json
{
  "id": "some-unique-identifier1",
  "tags": ["important", ".net"],
  "data": {
    "some": "data",
    "optional": "fields"
  }
}
```

Possible responses:

```text
201 Created
400 Bad Request
409 Conflict
```

`409 Conflict` is returned when a document with the same id already exists.

---

### Update document

```http
PUT /documents/{id}
Content-Type: application/json
```

Example request:

```json
{
  "id": "some-unique-identifier1",
  "tags": ["important", ".net", "updated"],
  "data": {
    "some": "updated data"
  }
}
```

Possible responses:

```text
204 No Content
400 Bad Request
404 Not Found
```

The route id must match the id in the request body.

---

### Get document

```http
GET /documents/{id}
Accept: application/json
```

Supported `Accept` headers:

```text
application/json
application/xml
application/x-msgpack
```

Possible responses:

```text
200 OK
404 Not Found
406 Not Acceptable
```

`406 Not Acceptable` is returned when the requested output format is not supported.

## How to Run

From the repository root:

```bash
dotnet run --project src/CODERAMA.Documents.API
```

Or run the `CODERAMA.Documents.API` project directly from Visual Studio.

Swagger is available in the development environment.

## How to Run Tests

From the repository root:

```bash
dotnet test
```

## Manual Testing

### Create a document

```bash
curl -X POST https://localhost:7231/documents \
  -H "Content-Type: application/json" \
  -d "{\"id\":\"document-1\",\"tags\":[\"important\",\".net\"],\"data\":{\"some\":\"data\"}}"
```

### Get document as JSON

```bash
curl https://localhost:7231/documents/document-1 \
  -H "Accept: application/json"
```

### Get document as XML

```bash
curl https://localhost:7231/documents/document-1 \
  -H "Accept: application/xml"
```

### Get document as MessagePack

```bash
curl https://localhost:7231/documents/document-1 \
  -H "Accept: application/x-msgpack" \
  --output document.msgpack
```

> Note: the port may be different depending on local Visual Studio launch settings.

## Architecture

The solution is divided into small, focused components.

### Controller Layer

`DocumentsController` handles HTTP requests and responses.

It is intentionally thin and delegates business logic to `IDocumentService`.

### Service Layer

`DocumentService` contains document-related business logic:

- create document
- reject duplicate document ids
- retrieve document by id
- update existing document
- validate route id and body id consistency

### Storage Layer

The service uses the `IDocumentStorage` abstraction.

Current implementation:

```text
InMemoryDocumentStorage
```

It is based on `ConcurrentDictionary`, which provides thread-safe access and is suitable for the assignment scenario where the load is expected to be high and mostly read-oriented.

To add another storage implementation, create a class implementing:

```csharp
IDocumentStorage
```

Examples:

```text
PostgreSqlDocumentStorage
AzureBlobDocumentStorage
AwsS3DocumentStorage
FileSystemDocumentStorage
RedisDocumentStorage
```

Then replace the dependency injection registration in `Program.cs`.

### Serialization Layer

The service uses the `IDocumentSerializer` abstraction.

Each output format has its own implementation:

```text
JsonDocumentSerializer
XmlDocumentSerializer
MessagePackDocumentSerializer
```

`DocumentSerializerResolver` selects the correct serializer based on the `Accept` header.

To add a new output format, create a new implementation of:

```csharp
IDocumentSerializer
```

Then register it in dependency injection.

No controller changes are required.

## Design Decisions

### Why separate storage behind an interface?

The assignment requires that it should be easy to add different underlying storage implementations such as cloud, HDD or in-memory storage.

Using `IDocumentStorage` keeps the API and business logic independent from the actual storage mechanism.

### Why separate serializers behind an interface?

The assignment requires that it should be easy to add support for new formats.

Using `IDocumentSerializer` allows adding new formats without changing the controller or service logic.

### Why use `JsonElement` for document data?

The schema of the `data` field can be arbitrary.

`JsonElement` allows the API to accept and store flexible JSON data without requiring a predefined schema.

### Why return `409 Conflict` on duplicate POST?

`POST /documents` is used to create a new document.

If a document with the same id already exists, returning `409 Conflict` makes the API behavior explicit and prevents accidental overwrites.

Updates are handled separately through `PUT /documents/{id}`.

## High Read Load Considerations

The assignment assumes high load, mostly reading.

Current implementation uses `ConcurrentDictionary` for thread-safe in-memory access.

For a real production environment, the following improvements could be added:

- persistent database or cloud storage
- distributed cache
- response caching
- rate limiting
- structured logging
- metrics and monitoring
- health checks
- horizontal scaling
- optimistic concurrency control
- authentication and authorization

## Tests

The test project covers the most important behavior:

- document creation
- duplicate document handling
- invalid input handling
- retrieving existing and missing documents
- updating existing documents
- update conflict between route id and body id
- storage behavior
- serializer resolver behavior

Run tests with:

```bash
dotnet test
```

## Future Improvements

Possible improvements for a real production version:

- persistent storage implementation
- integration tests for API endpoints
- OpenAPI response documentation
- request validation with FluentValidation
- authentication and authorization
- structured logging
- distributed caching
- CI/CD pipeline
- Docker support
- API versioning
- observability and metrics

## Notes

The current implementation uses in-memory storage for simplicity and demonstration purposes.

The main goal of the solution is to demonstrate clean structure, extensibility, testability and maintainable ASP.NET Core code.