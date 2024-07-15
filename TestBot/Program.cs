using JFA.Telegram.Console;
using Telegram.Bot;
using Telegram.Bot.Types;
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

        Console.WriteLine("Hello Avto Test Bot : ");

        var botManager = new TelegramBotManager();

        var bot = botManager.Create("7237808186:AAGnu416wExiX-yZw6XgZct4Dxj8B_mx67M");

        botManager.Start(BotFunction);


        void BotFunction(Update update)
        {
            var (chatId, username, message, chesk) = StaticService.GetData(update: update);

            if (chesk)
                return;

            var user = userService.AddUser(chatId, username);

            switch (user.UserStep)
            {
                case Step.AskName: AskName(user); break;
                case Step.SaveName: SaveName(user,message);break;
               case Step.SavePhoneNumber: SavePhoneNumber(user, update); break;
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
                AskPhoneNumber(user);
            }

            void AskPhoneNumber(User user, bool? check = false)
            {
                var buttoms = new List<List<KeyboardButton>>();

                var row = new List<KeyboardButton>()
                {
                    KeyboardButton.WithRequestContact("Sendt ur Contect")
                };
                buttoms.Add(row);
            
                var keybord = new ReplyKeyboardMarkup(buttoms) { ResizeKeyboard = true };
                
                user.UserStep = Step.SavePhoneNumber;
                userService.UpdateUsser();
                string text;
                
                if (chesk)
                {
                    text = "You  send wrong info, so send ur contect with tis button." +
                           " \n if u send,  you can go on"; 
                }
                else
                {
                    text = "Number : ";
                }
            
                bot.SendTextMessageAsync(user.ChatId, text , replyMarkup: keybord);
            }

            void SavePhoneNumber(User user, Update update)
            {
                string? number = update.Message?.Contact?.PhoneNumber;
                
                if(string.IsNullOrEmpty(number))
                 AskPhoneNumber(user, true);
                user.PhoneNumber = number;
                userService.UpdateUsser();
                ShowMenu(user);
            }

            void ShowMenu(User user)
            {
                var buttons = new List<List<KeyboardButton>>();
               
                var row1 = new  List<KeyboardButton>()
                {
                    new KeyboardButton("Tak a tesk \ud83d\udccb")
                };

                var row2 = new List<KeyboardButton>()
                {
                    new KeyboardButton("Show Result \ud83d\udcca"),
                    new KeyboardButton("Send message to admin 👨🏻‍💻")
                };

                var row3 = new List<KeyboardButton>()
                {
                    new KeyboardButton("About me ℹ️")
                };
                
                buttons.Add(row1);
                buttons.Add(row2);
                buttons.Add(row3);

                var keybord = new ReplyKeyboardMarkup(buttons) { ResizeKeyboard = true };
                
                user.UserStep = Step.ChooseMenu;
                userService.UpdateUsser();
                
                bot.SendTextMessageAsync(user.ChatId, "Menu : ", replyMarkup: keybord);
            }
        }
    }
}