using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

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
    public static Tuple<long, string?, string, int, bool, bool> GetData(Update update)
    {
        long chatId;
        string? username;
        string message;
        bool isPollAnswer;
        bool chesk;
        int messageId;

        if (update.Type == UpdateType.Message)
        {
            chatId = update.Message.From.Id;
            username = update.Message.From.Username;
            message = update.Message.Text;
            messageId = update.Message.MessageId;
            chesk = false;
            isPollAnswer = false;
        }
        else if (update.Type == UpdateType.CallbackQuery)
        {
            chatId = update.CallbackQuery!.From.Id;
            username = update.CallbackQuery.From.Username;
            message = update.CallbackQuery.Data!;
            messageId = update.CallbackQuery.Message.MessageId;
            chesk = false;
            isPollAnswer = false;
        }
        else if (update.Type == UpdateType.PollAnswer)
        {
            var answer = update.PollAnswer;
            chatId = answer.User.Id;
            username = answer.User.Username;
            var selectedId = answer.OptionIds[0];
            message = selectedId.ToString();
            messageId = 0;
            isPollAnswer = true;
            chesk = false; 
        }
        else
        {
            chatId = default;
            username = default;
            message = default;
            chesk = true;
            isPollAnswer = false;
            messageId = 0;
        }

        return new(chatId, username, message, messageId,isPollAnswer,chesk);
    }
  public  static bool CheckNumber(string text)
    {
        foreach (char  c  in  text)
        {
            if (!char.IsDigit(c))
            {
                return true;
            }
        }
        return false;
    }
  
   public static InlineKeyboardMarkup GetTickets()
    {
        var buttoms = new List<List<InlineKeyboardButton>>();
        var rows = new List<InlineKeyboardButton>();

        for (int i = 1; i < 36; i++)
        {
            var row = InlineKeyboardButton.WithCallbackData($"{i}");

            rows.Add(row);
            if (i % 7 == 0)
            {
                buttoms.Add(rows);
                rows = new();
            }
        }
        
        var keybord = new InlineKeyboardMarkup(buttoms);
        return keybord;
    }

    public static InlineKeyboardMarkup GetYerOrNo()
    {
        var buttons = new List<List<InlineKeyboardButton>>();

        var rows = new List<InlineKeyboardButton>()
        {
            InlineKeyboardButton.WithCallbackData("Yes"),
            InlineKeyboardButton.WithCallbackData("No")
        };
        buttons.Add(rows);

        var keybord = new InlineKeyboardMarkup(buttons);
        return keybord;
    }
}