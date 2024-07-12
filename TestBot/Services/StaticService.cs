using System.Text.Json.Serialization;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TestBot.Services;

public class StaticService
{
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
            message = update.CallbackQuery.Message.Text!;
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

}