public class TelegramService
{
    private readonly string _botToken = "BOT_TOKEN";
    private readonly HttpClient _httpClient;

    public TelegramService()
    {
        _httpClient = new HttpClient();
    }

    public async Task SendMessage(long chatId, string message)
    {
        var url = $"https://api.telegram.org/bot7917262516:AAF11XykA9EU5lsgmDxtGMh4DwphpuzUsak/sendMessage";
        var data = new Dictionary<string, string>
        {
            ["chat_id"] = chatId.ToString(),
            ["text"] = message
        };

        await _httpClient.PostAsync(url, new FormUrlEncodedContent(data));
    }
}
