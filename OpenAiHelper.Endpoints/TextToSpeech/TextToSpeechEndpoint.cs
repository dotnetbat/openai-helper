using FastEndpoints;
using OpenAiHelper.Integration.Api.Interfaces;

namespace OpenAiHelper.Endpoints.TextToSpeech;

public class TextToSpeechEndpoint(IAudioService audioService)
  : Endpoint<TextToSpeechRequest, TextToSpeechResponse>
{
  public override void Configure()
  {
    Post("text-to-speech");
    AllowAnonymous();
  }

  public override async Task HandleAsync(TextToSpeechRequest req, CancellationToken ct)
  {
    await audioService.VocalizeInServerFileAsync(req.Text, req.Name);
  }
}