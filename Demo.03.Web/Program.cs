using Demo.SematicKernel.Web;
using Microsoft.OpenApi;
using Microsoft.SemanticKernel;

var builder = WebApplication.CreateBuilder(args);

var kernelBuilder = Kernel.CreateBuilder();

builder.Services.AddControllers();

builder.Services.AddOpenApi();
//builder.Services.AddScoped<ChatService>();

builder.Services.AddLogging(b => b.AddConsole().SetMinimumLevel(LogLevel.Trace));

builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { Title = "Fundamentos AI Demo 02", Version = "v1" }));


var uri = new Uri("http://localhost:11434");

if (builder.Environment.IsDevelopment())
{
    kernelBuilder.AddOllamaChatCompletion(
        modelId: "llama3.2:1b",// supported models: llama3.2:1b
        endpoint: uri
    );
}
else
{
    var openAiKey =  Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? string.Empty;
    var openAiModel = builder.Configuration["AI_MODEL_GPT4"] ?? "gpt-4o-mini";

    kernelBuilder.AddOpenAIChatCompletion(
        modelId: openAiModel,
        apiKey: openAiKey
    );
}

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDistributedMemoryCache();
}
else
{
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = builder.Configuration.GetConnectionString("Redis");
        options.InstanceName = "ChatHistory:";
    });
}

var kernel = kernelBuilder.Build();

kernel.Plugins.AddFromType<ProductPlugin>("Products");

// Registrar no DI
builder.Services.AddSingleton(kernel);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
