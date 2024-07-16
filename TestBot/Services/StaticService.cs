using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TestBot.Services;

public static class StaticService
{
    public const string SendNameText = "Please , send ur name 🧑🏻";
    public const string  SendContectText = "Sendt ur Contect 📞";
    public const string TakeTestText = "Take a test \ud83d\udccb";
    public const string ShowResultText = "Show Result \ud83d\udcca";
    public const string MessageToAdminText = "Send message to admin 👨🏻‍💻";
    public const string AboutText = "About me ℹ️";
    public const string MenuText = "Menu 📖 : ";
    public static Tuple<long, string?, string, bool> GetData(Update update)
    {
        long chatId;
        string? username;
        string message;
        bool chesk;

        if (update.Type == UpdateType.Message)
        {
            chatId = update.Message.From.Id;
            username = update.Message.From.Username;
            message = update.Message.Text;
            chesk = false;
        }
        else if (update.Type == UpdateType.CallbackQuery)
        {
            chatId = update.CallbackQuery.Message.From.Id;
            username = update.CallbackQuery.Message.From.Username;
            message = update.CallbackQuery.Data!;
            chesk = false;
        }
        else
        {
            chatId = default;
            username = default;
            message = default;
            chesk = true;
        }

        return new(chatId, username, message, chesk);
    }
  public  static bool CheckNumber(string text)
    {
        for (int i = 0; i < text.Length; i++)
        {
            if (char.IsLetterOrDigit(text[i]))
            {
                return true;
            }
        }

        return false;
    }
}