using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TestBot.Entities;

namespace TestBot.Services;

public static class StaticService
{
    public const string SendNameText = "Please , send ur name 🧑🏻";
    public const string  SendContectText = "Sendt ur Contect 📞";
    public const string TakeTestText = "Take a test \ud83d\udccb";
    public const string ShowResultText = "Show Result \ud83d\udcca";
    public const string MessageToAdminText = "Send message to admin 👨🏻‍💻";
    public const string GetUsersMessage = " Get message from users 📨";
    public const string AboutText = "About me ℹ️";
    public const string AnalyzeTicket = "Analyze Ticket Questions 📑";
    public const string ChangeAboutText = "Change Info ℹ️";
    public const string AddTest = "Add Test ✎";
    public const string DeleteTest = "Delete Test 🗑️";
    public const string AddAdmin = "Add Admin ➕";
    public const string RemoveAdmin = "Remove Admin ⛔";
    public const string SendTextAdd = " Send  Text Ads 📣";
    public const string SendFullAds = "Send Full Ads 📣";
    public const string GetAllUsers = "Get All Users 🙋🏻";
    public const string AddChannelLink = " Add channel Link 🎯";
    public const string DeleteChenelLink = "Delete Chenel Link 📌";
    public const string MenuText = "Menu 📖 : ";
    public const string BackText = "Back 🔙";
    public const long SuperAdmin = 1795525299;
    
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

    public static InlineKeyboardMarkup GetYerOrNo(int ticketId)
    {
        var buttons = new List<List<InlineKeyboardButton>>();

        var rows = new List<InlineKeyboardButton>()
        {
            InlineKeyboardButton.WithCallbackData("Yes", $"yes,{ticketId}"),
            InlineKeyboardButton.WithCallbackData("No", $"no,{ticketId}")
        };
        buttons.Add(rows);

        var keybord = new InlineKeyboardMarkup(buttons);
        return keybord;
    }

    public static ReplyKeyboardMarkup GetUserMenu()
    {
        var buttons = new List<List<KeyboardButton>>();

        var row1 = new List<KeyboardButton>()
        {
            new (TakeTestText)
        };

        var row2 = new List<KeyboardButton>()
        {
            new (ShowResultText),
            new (MessageToAdminText)
        };

        var row3 = new List<KeyboardButton>()
        {
            new (AboutText) 
        };

        buttons.Add(row1);
        buttons.Add(row2);
        buttons.Add(row3);

         return  new ReplyKeyboardMarkup(buttons) { ResizeKeyboard = true };
        
    }

    public static ReplyKeyboardMarkup GetAdminMenu()
    {
        var buttons = new List<List<KeyboardButton>>();

        var row1 = new List<KeyboardButton>()
        {
            new (TakeTestText)
        };

        var row2 = new List<KeyboardButton>()
        {
            new (ShowResultText),
            new (GetUsersMessage)
        };

        var row3 = new List<KeyboardButton>()
        {
            new (ChangeAboutText) 
        };
        var row4 = new List<KeyboardButton>()
        {
             new (AddTest),
             new(DeleteTest)
        };
        
        buttons.Add(row1);
        buttons.Add(row2);
        buttons.Add(row3);
        buttons.Add(row4);

        return  new ReplyKeyboardMarkup(buttons) { ResizeKeyboard = true };
    }

    public static ReplyKeyboardMarkup GetSuperAdminMenu()
    {
        var buttons = new List<List<KeyboardButton>>();

        var row1 = new List<KeyboardButton>()
        {
            new (TakeTestText),
            new (ShowResultText)
        };

        var row2 = new List<KeyboardButton>()
        {
            new (GetUsersMessage),
            new (ChangeAboutText)
        };

        var row3 = new List<KeyboardButton>()
        {
            new (AddTest),
            new(DeleteTest)
        };
        var row4 = new List<KeyboardButton>()
        {
            new(AddAdmin),
            new(RemoveAdmin)
        };
        var row5 = new List<KeyboardButton>()
        {
            new(SendTextAdd),
            new(SendFullAds)
        };
        var row6 = new List<KeyboardButton>()
        {
            new(GetAllUsers),
            new(AddChannelLink),
            new(DeleteChenelLink)
        };
        buttons.Add(row1);
        buttons.Add(row2);
        buttons.Add(row3);
        buttons.Add(row4);
        buttons.Add(row5);
        buttons.Add(row6);

        return  new ReplyKeyboardMarkup(buttons) { ResizeKeyboard = true };
    }

    public static ReplyKeyboardMarkup Back()
    {
        var buttoms = new List<List<KeyboardButton>>();

        var rows = new List<KeyboardButton>()
        {
            new KeyboardButton(BackText)
        };
        
        buttoms.Add(rows);

        return new ReplyKeyboardMarkup(buttoms) { ResizeKeyboard = true };
    }
  public static string ResultMessage(string firstname, Ticket ticket)
    {
        var quality = (ticket?.Result?.CorrecAnswerCount * 1.0 / ticket?.Result?.TotalAnswerCount) * 100;
        return
            "📝 Natijangiz: \r\n " +
            $"👨🏻‍💼 Foydalanuvchi : {firstname} \r\n" +
            $"💻 Ticket Raqam :{ticket.Id} \r\n" +
            $"✅ Togri Javoblar : {ticket.Result.CorrecAnswerCount} ta\r\n" +
            $"❌ Notog'ri Javoblar : {ticket.Result.InCorrectAnswerCount} ta\r\n" +
            $"📊 Sifat : {quality}%\r\n" +
            $"📆 {ticket.TookAt:d} ⏰ {ticket.TookAt:t}\r\n" +
            "\r\n------------------------\r\n";
    }
}