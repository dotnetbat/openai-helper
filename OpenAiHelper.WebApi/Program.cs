using FastEndpoints;
using OpenAi.Integration.Api;
using OpenAiHelper.Endpoints.TextToSpeech;
using OpenAiHelper.WebApi;
using OpenAiHelper.WebApi.TelegramBot;
using TelegramBot.Integration.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddYamlFile("appsettings.yaml", optional: false, reloadOnChange: true);

if (builder.Environment.IsDevelopment())
{
  builder.Configuration.AddUserSecrets<Program>();
}

builder.Configuration.AddEnvironmentVariables();

builder.AddOpenAi();
builder.AddTelegramBot();
builder.AddTelegramBotMessageHandler();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddFastEndpoints(options =>
{
  options.Assemblies = new[] { typeof(TextToSpeechEndpoint).Assembly };
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseFastEndpoints();

app.UseTelegramBotMessageHandler();

app.Run();