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
   public static void Main(string[] args)
    {
        
        UserService userService = new();
        TestService testService = new();
        TicketService ticketService = new();

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
               case Step.ChooseMenu: ChooseMenu(user, message);break;
               case Step.ChooseTicket: SaveTicket(user,message); break;
               case Step.YesOrNo: break;
            }
        }
            void AskName(User user)
            {
                var text = StaticService.SendNameText;
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

            void AskPhoneNumber(User user, bool check = false)
            {
                var buttoms = new List<List<KeyboardButton>>();

                var row = new List<KeyboardButton>()
                {
                    KeyboardButton.WithRequestContact(StaticService.SendContectText)
                };
                buttoms.Add(row);
            
                var keybord = new ReplyKeyboardMarkup(buttoms) { ResizeKeyboard = true };
                
                user.UserStep = Step.SavePhoneNumber;
                userService.UpdateUsser();
                var text = check ? "You sent wrong info, so send ur contact with this button. \n If u send, u can go on" : "Number :";

                bot.SendTextMessageAsync(user.ChatId, text , replyMarkup: keybord);
            }

            void SavePhoneNumber(User user, Update update)
            {
                string? number = update.Message?.Contact?.PhoneNumber;
                
                if(string.IsNullOrEmpty(number))
                 AskPhoneNumber(user, true);
                else
                {
                    user.PhoneNumber = number;
                    userService.UpdateUsser();
                    ShowMenu(user);
                }
            }

            void ShowMenu(User user)
            {
                var buttons = new List<List<KeyboardButton>>();
               
                var row1 = new  List<KeyboardButton>()
                {
                    new KeyboardButton(StaticService.TakeTestText)
                };

                var row2 = new List<KeyboardButton>()
                {
                    new KeyboardButton(StaticService.ShowResultText),
                    new KeyboardButton(StaticService.MessageToAdminText)
                };

                var row3 = new List<KeyboardButton>()
                {
                    new KeyboardButton(StaticService.AboutText)
                };
                
                buttons.Add(row1);
                buttons.Add(row2);
                buttons.Add(row3);

                var keybord = new ReplyKeyboardMarkup(buttons) { ResizeKeyboard = true };
                
                user.UserStep = Step.ChooseMenu;
                userService.UpdateUsser();
                
                bot.SendTextMessageAsync(user.ChatId, StaticService.MenuText, replyMarkup: keybord);
            }

            void ChooseMenu(User user, string message)
            {
                switch (message)
                {
                    case StaticService.TakeTestText : ShowTicket(user); break;
                    case StaticService.ShowResultText: break;
                    case StaticService.MessageToAdminText: break;
                    case StaticService.AboutText: break;
                    default: ShowMenu(user); break;
                }
            }

            void ShowTicket(User user)
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
                bot.SendTextMessageAsync(user.ChatId, "Choose one of these ticket in order to take a test :)",replyMarkup: keybord);
            }

            void SaveTicket(User user, string message)
            {
                var check = StaticService.CheckNumber(message);
                if (check) 
                    TellAboutError(user);

                int intNumber= int.Parse(message);
                
                if(!(intNumber is > 0 and < 36))
                    TellAboutError(user);
                
                byte tickedId = Convert.ToByte(intNumber);

                var ticket = ticketService.AddOrUpdate(user.ChatId, tickedId);

                if (ticket.Result is not null)
                    TellAboutResult(user,ticket.Result);

                for (int i = 20*(tickedId -1) + 1 ; i <= tickedId*20; i++)
                {
                    SendTest(user:user,testId:i);
                }
            }

            void SendTest(User user, int testId)
            {
                var test = testService.Tests.Find(x => x.Id == testId);

                List<string> options = new();

                int correctId = 0;
                
                for(int i = 0; i < test?.Choices.Count; i++)
                {
                    options.Add(test.Choices[i].Text);
                    
                    if (test.Choices[i].Answer)
                    {
                        correctId = i ;
                    }
                }

                bot.SendPollAsync(user.ChatId, test!.Question,
                    options: options, 
                    correctOptionId: correctId,
                    isAnonymous: false,
                    type:PollType.Quiz,
                    closeDate:DateTime.Now.AddMinutes(1));
            } 
            void TellAboutResult(User user, Result result)
            {
                var text = "You took this ticket before. \n" +
                           $"Total Question count is {result.TotalAnswerCount}\n" +
                           $"Your Correct answers count is {result.CorrecAnswerCount}\n" +
                           $"Your InCorrect answers count is {result.InCorrectAnswerCount}";

                var buttons = new List<List<InlineKeyboardButton>>();

                var rows = new List<InlineKeyboardButton>()
                {
                    InlineKeyboardButton.WithCallbackData("Yes"),
                    InlineKeyboardButton.WithCallbackData("No")
                };
                buttons.Add(rows);
                
                var keybord = new InlineKeyboardMarkup(buttons);
                
                user.UserStep = Step.YesOrNo;
                userService.UpdateUsser();
                
                bot.SendTextMessageAsync(user.ChatId, text, replyMarkup: keybord); 

            }

            void TellAboutError(User user)
            {
                var text = "u send wrong info, if u wanna take a test , please choose ticket with these buttons. " +
                           "\n  Don't send anything else :)";
               
                bot.SendTextMessageAsync(user.ChatId, text);
                ShowTicket(user);
            }

           
            
            
            
            
            
        
    }
}