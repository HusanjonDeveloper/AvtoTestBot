using JFA.Telegram.Console;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TestBot.Entities;
using TestBot.Services;
using User = TestBot.Entities.User;

namespace TestBot;

class Program
{
    static void Main(string[] args)
    {
        UserService userService = new();
        TestService testService = new();
        
        Console.WriteLine("Hello World");

        var botManager = new TelegramBotManager();

        var bot = botManager.Create("7407824827:AAGi6MoDU7BtuanV8-t6N88HNLNLzxGa8-Q");
        
        botManager.Start(BotFunction);
         
          
        void BotFunction(Update update)
        {
            var (chatId, username, message, chesk) = StaticService.GetData(update: update);

            if (chesk)
                return;

            var user = userService.AddUser(chatId, username);

            /*switch (user.UserStep)
            {
                case Step.AskName: AskName(user); break;
                case Step.SaveName: SaveName(user,message);break;
              //  case Step.SavePhoneNumber: SavePhoneNumber(); break;
            }
            */
            
            var buttoms = new List<List<KeyboardButton>>();
            var rows = new List<KeyboardButton>()
            {
                KeyboardButton.WithRequestContact("send ur contect")
            };

            if (!string.IsNullOrEmpty(update.Message?.Contact?.PhoneNumber))
            {
                Console.WriteLine(update.Message.Contact.PhoneNumber);
            }
            else
            {
                Console.WriteLine("Contect is Null");
            }
            buttoms.Add(rows);
            
           
            var keybord = new ReplyKeyboardMarkup(buttoms){ResizeKeyboard = true};
            bot.SendTextMessageAsync(user.ChatId, "Menu", replyMarkup:keybord);
        }

        void AskName(User user)
        {
            var text = "Please , send ur name";
            user.UserStep = Step.SaveName;
            userService.UpdateUsser();
            bot.SendTextMessageAsync(user.ChatId, text);
        }

        void SaveName(User user, string message)
        {
            user.FirstName = message;
            user.UserStep = Step.SavePhoneNumber;
            userService.UpdateUsser();
        }
    }
}