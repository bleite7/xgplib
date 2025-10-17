using Serilog;

// Configure Serilog for logging
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddProblemDetails();
builder.Services.AddSyncServiceDependencies(builder.Configuration);

// Add Serilog for logging
builder.Services.AddSerilog();

// Add NSwag services
builder.Services.AddOpenApiDocument(config =>
{
    config.Title = "XgpLib.SyncService.Api";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
}
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
