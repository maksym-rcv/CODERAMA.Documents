using CODERAMA.Documents.API.Serialization;
using CODERAMA.Documents.API.Services;
using CODERAMA.Documents.API.Storage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IDocumentStorage, InMemoryDocumentStorage>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddSingleton<IDocumentSerializer, JsonDocumentSerializer>();
builder.Services.AddSingleton<IDocumentSerializer, XmlDocumentSerializer>();
builder.Services.AddSingleton<IDocumentSerializer, MessagePackDocumentSerializer>();
builder.Services.AddSingleton<IDocumentSerializerResolver, DocumentSerializerResolver>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
