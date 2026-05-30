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
- Unit-testable business logic

## Tech Stack

- .NET 8
- ASP.NET Core Web API
- C#
- MessagePack

## API Endpoints

### Create document

POST /documents
Content-Type: application/json
{
  "id": "some-unique-identifier1",
  "tags": ["important", ".net"],
  "data": {
    "some": "data",
    "optional": "fields"
  }
}

### Update document

PUT /documents/{id}
Content-Type: application/json

### Get document

GET /documents/{id}
Accept: application/json

### Supported Accept headers

application/json
application/xml
application/x-msgpack

## Architecture

The solution is intentionally small but structured for extensibility.

Main components:

DocumentsController handles HTTP requests and responses.
DocumentService contains document-related business logic.
IDocumentStorage abstracts the storage layer.
InMemoryDocumentStorage provides a thread-safe in-memory implementation.
IDocumentSerializer abstracts response serialization.
JSON, XML and MessagePack serializers are implemented separately.
DocumentSerializerResolver selects the correct serializer based on the Accept header.
