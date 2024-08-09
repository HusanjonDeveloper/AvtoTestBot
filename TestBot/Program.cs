using JFA.Telegram.Console;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
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

        var bot = botManager.Create("7407824827:AAGi6MoDU7BtuanV8-t6N88HNLNLzxGa8-Q");

        botManager.Start(BotFunction);
        return;


        void BotFunction(Update update)
        {
            var (chatId, username, message, messageId, isPollAnswer, chesk) = StaticService.GetData(update: update);

            if (chesk)
                return;

            var user = userService.AddUser(chatId, username);

            if (isPollAnswer)
            {
                int selectedId = int.Parse(message);
                Sending(user, selectedId);
            }
            else
            {
                Console.WriteLine(message);
                var isProcessing = CheckForProcessing(user);

                if (isProcessing)
                {
                    bot.SendTextMessageAsync(user.ChatId, "Currently, You're taking test and you have not finished yes it, so go on");
                }
                else
                {
                    switch (user.UserStep)
                    {
                        case Step.AskName: AskName(user); break;
                        case Step.SaveName: SaveName(user, message); break;
                        case Step.SavePhoneNumber: SavePhoneNumber(user, update); break;
                        case Step.ChooseMenu: ChooseMenu(user, message); break;
                        case Step.ChooseTicketForTest: SaveTicket(user, message, messageId); break;
                        case Step.ChooseTicketForResult : ShowResultById(user, message, messageId); break;
                        case Step.YesOrNo: break;
                    }   
                }
            }
        }

        bool CheckForProcessing(User user)
        {
            return user.TicketInfo is not null;
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
            var text = check
                ? "You sent wrong info, so send ur contact with this button. \n If u send, u can go on"
                : "Number :";

            bot.SendTextMessageAsync(user.ChatId, text, replyMarkup: keybord);
        }

        void SavePhoneNumber(User user, Update update)
        {
            string? number = update.Message?.Contact?.PhoneNumber;

            if (string.IsNullOrEmpty(number))
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

            var row1 = new List<KeyboardButton>()
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
            try
            {
                switch (message)
                {
                    case StaticService.TakeTestText:ShowTicket(user);break;
                    case StaticService.ShowResultText: ShowResults(user); break;
                    case StaticService.MessageToAdminText: break;
                    case StaticService.AboutText: break;
                    default:ShowMenu(user);break;
                }
            }
            catch (Exception e)
            {
                ShowMenu(user);
            }
        }
        
        void ShowTicket(User user)
        {
            var keybord = StaticService.GetTickets();
            user.UserStep = Step.ChooseTicketForTest;
            userService.UpdateUsser();

            bot.SendTextMessageAsync(user.ChatId, "Choose one of these ticket in order to take a test :)",
                replyMarkup: keybord);
        }

        //async void Info(User user)
        
        void SaveTicket(User user, string message, int messageId)
        {
            bot.DeleteMessageAsync(user.ChatId, messageId);
            var (ticket, tickedId, check) = GetTicket(user, message);
           
            if(!check)
                return;
            
            if (ticket.Result is not null)
            {
                TellAboutResult(user, ticket);
                return;
            }

            user.TicketInfo = new()
            {
                NextTestId = 20 * (tickedId - 1) + 1,
                EndTo = tickedId * 20,
                TicketId = tickedId
            };
            userService.UpdateUsser();
            SendTest(user);
        }

        void Sending(User user, int selectedId)
        {
            if (user.TicketInfo is null)
                return;

            var ticket = ticketService.AddOrUpdate(user.ChatId, user.TicketInfo.TicketId);
            var test = testService.Tests.Find(t => t.Id == user.TicketInfo.NextTestId - 1);

            ticket.Result ??= new() { CorrecAnswerCount = 0 };

                if (test!.Choices[selectedId].Answer)
                {
                    ticket.Result.CorrecAnswerCount += 1;
                }   

            ticketService.UpdateTicket();

            if (user.TicketInfo.IsCompleted)
            {
                ShowResult(user, ticket!);
                user.TicketInfo = null;
                userService.UpdateUsser();
                ShowMenu(user);
            }
            else
            {
                SendTest(user);
            }
        }

        async void SendTest(User user)
        {
            if (user.TicketInfo is null)
                return;

            var test = testService.Tests.Find(x => x.Id == user.TicketInfo.NextTestId);
            var question = $"{test!.Id}. {test.Question}";

            int characterNumber = 65;
            List<string> options = new();

            int correctId = 0;

            for (int i = 0; i < test?.Choices.Count; i++)
            {
                var letter = Convert.ToChar(characterNumber); // A/ B/ C in ASCII table
                question += $"\n {letter} ) {test.Choices[i].Text}";
                options.Add($"{letter}");

                if (test.Choices[i].Answer)
                {
                    correctId = i;
                }

                characterNumber++;
            }

            user.TicketInfo.NextTestId += 1;
            userService.UpdateUsser();

            if (test!.Media.Exist)
            {
                
                var path = $"Autotest/{test.Media.Name}.png";
                var data = await System.IO.File.ReadAllBytesAsync(path);
                var ms = new MemoryStream(data);
                var photo = new InputOnlineFile(ms);

                await bot.SendPhotoAsync(user.ChatId, caption: question, photo: photo);
            }
            else
            {
                await bot.SendTextMessageAsync(user.ChatId, text: question);
            }

            await bot.SendPollAsync(user.ChatId,
                question: "question",
                options: options,
                correctOptionId: correctId,
                isAnonymous: false,
                type: PollType.Quiz,
                closeDate: DateTime.Now.AddSeconds(10));
           SendTest(user);
            //IsClosed(user);
        }

        /*
      async  void IsClosed(User user)
        {
          await Task.Delay(12000 );
            Sending(user,selectedId: -1);
        }*/

        void ShowResults(User user)
        {
            var keybord =  StaticService.GetTickets();
            user.UserStep = Step.ChooseTicketForResult;
            userService.UpdateUsser();

            bot.SendTextMessageAsync(user.ChatId, "Choose one of these ticket in order to take a test :)",
                replyMarkup: keybord);
            
        }

        void ShowResultById(User user, string message, int messageId)
        {
            bot.DeleteMessageAsync(user.ChatId, messageId);
            var (ticket, ticketId, check) = GetTicket(user, message);
            
            if(!check)
                return;
            
            ShowResult(user,ticket);
            ShowMenu(user);
        }
        
        void ShowResult(User user, Ticket? ticket)
        {
            if (ticket is null)
            {
                NotFoundTicket(user);
            }
            else
            {
                var message = StaticService.ResultMessage(user.FirstName,ticket);

                bot.SendTextMessageAsync(user.ChatId, message);   
            }
        }

        void TellAboutResult(User user, Ticket ticket)
        {

            var keybord = StaticService.GetYerOrNo();
            var message = StaticService.ResultMessage(user.FirstName,ticket);
            user.UserStep = Step.YesOrNo;
            userService.UpdateUsser();

            bot.SendTextMessageAsync(user.ChatId, message, replyMarkup: keybord);
        }
        

        void TellAboutError(User user)
        {
            var text = "u send wrong info, if u wanna take a test , please choose ticket with these buttons. " +
                       "\n  Don't send anything else :)";

            bot.SendTextMessageAsync(user.ChatId, text);
            ShowTicket(user);
        }

        void NotFoundTicket(User user)
        {
            var message = "You did't take this ticket before";
            
            var keybord = StaticService.GetYerOrNo();

            bot.SendTextMessageAsync(user.ChatId, message, replyMarkup: keybord);
        }

        Tuple<Ticket,byte, bool> GetTicket(User user,string message)
        {
            var check = StaticService.CheckNumber(message);
            if (check)
            {
                TellAboutError(user);
                return new(null, 0, false);
            }

            int intNumber = int.Parse(message);

            if (!(intNumber is > 0 and < 36))
                TellAboutError(user);

            byte tickedId = Convert.ToByte(intNumber);

            var ticket = ticketService.AddOrUpdate(user.ChatId, tickedId);
            return new(ticket, tickedId, true);
        }
        
        
        
        
        
    }
}