using OpenAi.Integration.Api.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Integration.Api;

namespace OpenAiHelper.WebApi.TelegramBot;

public class TelegramBotMessageHandler
{
  private readonly ITelegramService _telegramService;
  private readonly IAudioService _audioService;

  public TelegramBotMessageHandler(ITelegramService telegramService, IAudioService audioService)
  {
    _telegramService = telegramService;
    _audioService = audioService;
    telegramService.MessageHandler = HandleAsync;
  }
  
  private async Task HandleAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
  {
    if (update is { Type: UpdateType.Message, Message.Text: not null })
    {
      switch (update.Message.Text.ToLower())
      {
        case "/start":
          var replyKeyboard = new ReplyKeyboardMarkup(new[]
          {
            new KeyboardButton[] { "Text to speech" }
          })
          {
            ResizeKeyboard = true, // Optional: to fit the keyboard to button sizes
            OneTimeKeyboard = true, // Optional: to hide the keyboard after one use
          };

          await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Welcome! Choose an option:", replyMarkup: replyKeyboard, cancellationToken: ct);
          break;

        case "text to speech":
          await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Send me the text", cancellationToken: ct);
          _telegramService.ChatsInWork.TryAdd(update.Message.Chat.Id.ToString(), "text-to-speech");
          break;

        default:
          if (_telegramService.ChatsInWork.TryGetValue(update.Message.Chat.Id.ToString(), out var operation) && operation == "text-to-speech")
          {
            var audioBytes = await _audioService.VocalizeAsync(update.Message.Text);
            
            _telegramService.ChatsInWork.TryRemove(update.Message.Chat.Id.ToString(), out _);
            
            var fileName = update.Message.Chat.Id + " - " + DateTime.UtcNow.ToString("yyyy-MM-dd HH-mm-ss") + ".mp3";
            var audioFile = new InputFileStream(new MemoryStream(audioBytes), fileName);

            await botClient.SendAudioAsync(update.Message.Chat.Id, audioFile, cancellationToken: ct);
          }
          else
          {
            await botClient.SendTextMessageAsync(update.Message.Chat.Id, "I don't understand you", cancellationToken: ct);
          }
          break;
      }
    }
  }
}