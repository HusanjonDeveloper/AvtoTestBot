using System.Net;
using JFA.Telegram.Console;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using TestBot.Entities;
using TestBot.Services;
using File = System.IO.File;
using User = TestBot.Entities.User;

namespace TestBot;
class Program
{
    public static void Main(string[] args)
    {
        UserService userService = new();
        TestService testService = new();
        TicketService ticketService = new();
        InfoService infoService = new();

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
            else if(message == StaticService.BackText)
            {
                 ShowMenu(user);
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
                        case Step.YesOrNo: YesOrNo( user, message,messageId);break;
                        case Step.ChooseTicketForAnalyze: ChooseTicketForAnalyze(user, message, messageId); break;
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
            userService.UpdateUser();
            bot.SendTextMessageAsync(user.ChatId, text);
        }

        void SaveName(User user, string message)
        {
            user.FirstName = message;
            user.UserStep = Step.SavePhoneNumber;
            userService.UpdateUser();
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
            userService.UpdateUser();
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
                userService.UpdateUser();
                ShowMenu(user);
            }
        }

        void ShowMenu(User user)
        {
            user.UserStep = Step.ChooseMenu;
            userService.UpdateUser();
            ReplyKeyboardMarkup keybord;
               
            switch (user.Role)
            {
                case UserRole.User: keybord = StaticService.GetUserMenu(); break;
                case UserRole.Admin:keybord = StaticService.GetAdminMenu(); break;
                case UserRole.SuperAdmin:keybord = StaticService.GetSuperAdminMenu();break;
                default: keybord = StaticService.GetUserMenu(); break;
            }
            
            bot.SendTextMessageAsync(user.ChatId, "Menu", replyMarkup: keybord);
        }
        
        void ChooseMenu(User user, string message) 
        {
            switch (user.Role)
            {
                case UserRole.User: ChooseUserMenu(user,message); break;
                case UserRole.Admin: ChooseAdminMenu(user,message); break;
                case UserRole.SuperAdmin: ChooseSuperAdminMenu(user,message); break;
                default: ChooseUserMenu(user,message); break;
            }
        }

        void ChooseUserMenu(User user, string message)
        {
            try
            {
                switch (message)
                {
                    case StaticService.TakeTestText:ShowTicket(user);break;
                    case StaticService.ShowResultText: ShowResults(user); break;
                    case StaticService.MessageToAdminText: break;
                    case StaticService.AboutText: Info(user); break;
                    case StaticService.AnalyzeTicket: ShowTicketForAnalyze(user); break;
                    default:ShowMenu(user);break;
                }
            }
            catch (Exception e)
            {
                ShowMenu(user);
            }
        }

        void ChooseAdminMenu(User user, string message)
        {
            
        }

        void ChooseSuperAdminMenu(User user, string message)
        {
            
        }

      async  void Info(User user)
        {
            var data = File.ReadAllBytes(infoService.Info.PhotoUrl);
            var ms = new MemoryStream(data);
            var photo = new InputOnlineFile(ms);

            await bot.SendPhotoAsync(user.ChatId, photo: photo, caption: infoService.Info.Text);
            ShowMenu(user);
        }
        
       async void ShowTicket(User user)
        {
            var keybord = StaticService.GetTickets();
            user.UserStep = Step.ChooseTicketForTest;
            userService.UpdateUser();

          await  bot.SendTextMessageAsync(user.ChatId, "Choose one of these ticket in order to take a test :)",
              replyMarkup: keybord);
              
          SendingBack(user);
        }
       
        //async void Info(User user)
        
        void SaveTicket(User user, string message, int messageId)
        {
            bot.DeleteMessageAsync(user.ChatId, messageId);
            var (ticket, ticketId, check) = GetTicket(user, message);
           
            if(!check)
                return;
            
            if (ticket.Result is not null)
            {
                TellAboutResult(user, ticket);
                return;
            }

            TicketInfoAndTest(user, ticketId);
        }

        void TicketInfoAndTest(User user, byte ticketId)
        {
            
            user.TicketInfo = new()
            {
                NextTestId = 20 * (ticketId - 1) + 1,
                EndTo = ticketId * 20,
                TicketId = ticketId
            };
            userService.UpdateUser();
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
                userService.UpdateUser();
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
            userService.UpdateUser();

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
          await Task.Delay(12000);
            Sending(user,selectedId: -1);
        }*/

      async  void ShowResults(User user)
        {
            var keybord =  StaticService.GetTickets();
            user.UserStep = Step.ChooseTicketForResult;
            userService.UpdateUser();

           await bot.SendTextMessageAsync(user.ChatId, "Choose one of these ticket in order to take a test :)",
                replyMarkup: keybord);
            
            SendingBack(user);
            
        }

        void ShowResultById(User user, string message, int messageId)
        {
            bot.DeleteMessageAsync(user.ChatId, messageId);
            var (ticket, ticketId, check) = GetTicket(user, message);
            
            if(!check)
                return;
            
            ShowResult(user,ticket);
        }
        
        void ShowResult(User user, Ticket ticket)
        {
            if ( ticket.Result is null)
            {
                NotFoundTicket(user,ticket.Id);
            }
            else
            {
                var message = StaticService.ResultMessage(user.FirstName,ticket);

                bot.SendTextMessageAsync(user.ChatId, message);   
                ShowMenu(user);
            }
        }

        void TellAboutResult(User user, Ticket ticket)
        {

            var keybord = StaticService.GetYerOrNo(ticket.Id);
            var message = StaticService.ResultMessage(user.FirstName,ticket);
            user.UserStep = Step.YesOrNo;
            userService.UpdateUser();

            bot.SendTextMessageAsync(user.ChatId, message, replyMarkup: keybord);
        }
        
         async void TellAboutError(User user)
        {
            var text = "u send wrong info, if u wanna take a test , please choose ticket with these buttons. " +
                       "\n  Don't send anything else :)";

           await bot.SendTextMessageAsync(user.ChatId, text);
            ShowTicket(user);
        }

       async void NotFoundTicket(User user, int ticketId)
        {
            var message = "You did't take this ticket before " +
                          "\n Do you wanna take this ticket now?";
            
            var keybord = StaticService.GetYerOrNo(ticketId);
            user.UserStep = Step.YesOrNo;
            userService.UpdateUser();
            
           await bot.SendTextMessageAsync(user.ChatId, message, replyMarkup: keybord);
            SendingBack(user);
        }

      async  void YesOrNo(User user, string message, int messageId)
      {
          var text = "u sent wrong answer for this action !" +
                     "\n please send answer by using this vuttons";

          if (!message.Contains(','))
          {
              await bot.SendTextMessageAsync(user.ChatId, text);
              return;
          }
              
          var data = message.Split(',').ToArray();
          
            if (!(data[0] == "yes" || data[0] == "no"))
            {
                await bot.SendTextMessageAsync(user.ChatId, text);
                return;
            }
            
            if (data[0] == "yes")
            {
                var ticketId = Convert.ToByte(data[1]);
                var ticket = ticketService.Tickets.Find(x => x.Id == ticketId);
                ticketService.Tickets.Remove(ticket!);
                
                 ticketService.UpdateTicket();
               TicketInfoAndTest(user,ticketId);
            }
            else
            {
                ShowMenu(user);
            }

            await bot.DeleteMessageAsync(user.ChatId, messageId);
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
            {
                TellAboutError(user);
                return new(null, 0, false);
            }

            byte tickedId = Convert.ToByte(intNumber);

            var ticket = ticketService.AddOrUpdate(user.ChatId, tickedId);
            return new(ticket, tickedId, true);
        }

     async  void ShowTicketForAnalyze(User user)
        {
            var keybord = StaticService.GetTickets();
            user.UserStep = Step.ChooseTicketForAnalyze;
            userService.UpdateUser();

            await  bot.SendTextMessageAsync(user.ChatId, "Choose one of these ticket in order to Analyze Ticket:)",
                replyMarkup: keybord);
              
            SendingBack(user);
        }

     async void ChooseTicketForAnalyze(User user, string message, int messageId)
        {
           await bot.DeleteMessageAsync(user.ChatId, messageId);
            var (ticket, ticketId, check) = GetTicket(user, message);
           
            if(!check)
                return;
            
            var startIndex = 20 * (ticket.Id - 1) + 1;
            var endIndex = 20 * ticket.Id;
            
            var tests = testService.Tests.
                Where(x => x.Id >= 
                    startIndex && x.Id <= endIndex)
                .ToList();

            foreach (var test in tests)
            {
                string correctOption = "";
                foreach (var choice in  test.Choices)
                {
                    if (choice.Answer)
                        correctOption = choice.Text;
                }
                
                var text = $"📌 {test.Id}. {test.Question} \n" +
                           $"\n ✅ Correct option : \n {correctOption} \n" +
                           "\n 💡 Description for this correct option : \n" +
                           $"{test.Description}";

                if (test.Media.Exist)
                {
                    var path = $"Autotest/{test.Media.Name}.png";
                    var data = File.ReadAllBytes(path);
                    var ms = new MemoryStream(data);
                    var photo = new InputOnlineFile(ms);
                  await  bot.SendPhotoAsync(user.ChatId, photo, caption: text);
                }
                else
                {
                    await bot.SendTextMessageAsync(user.ChatId, text);
                }
            }
            ShowMenu(user);
        }
        
        async void SendingBack(User user)
        {
            var back = StaticService.Back();
            await bot.SendTextMessageAsync(user.ChatId, "Menu", replyMarkup:back);
        }

       async void SendMessageToAdmin(User user)
        {
            var text = "Write message for admin";
            user.UserStep = Step.SaveMessageForAdmin;
            userService.UpdateUser();
            await bot.SendTextMessageAsync(user.ChatId, text);
            SendingBack(user);
        }

       /*
       async void SaveMessageForAdmin(User user, string message)
        {
            applicationService.AddApplication(user, message);
            var text = "Ur message was sent to admin. Later, Admin will contact with u";
            await bot.SendTextMessageAsync(user.ChatId, text);
            ShowMenu(user);
        }
        */

        void ShowChangingInfo(User user)
        {
            
        }
       
       
       
       
    }
}