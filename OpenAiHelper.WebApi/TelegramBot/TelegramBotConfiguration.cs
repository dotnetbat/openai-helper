namespace OpenAiHelper.WebApi.TelegramBot;

public static class TelegramBotConfiguration
{
  public static void AddTelegramBotMessageHandler(this WebApplicationBuilder builder)
  {
    builder.Services.AddSingleton<TelegramBotMessageHandler>();
  }

  public static void UseTelegramBotMessageHandler(this WebApplication app)
  {
    app.Services.GetRequiredService<TelegramBotMessageHandler>();
  }
}