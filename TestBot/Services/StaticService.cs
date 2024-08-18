
using OfficeOpenXml;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TestBot.Entities;

namespace TestBot.Services;

public static class StaticService
{
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

    public static ReplyKeyboardMarkup Back()
    {
        var buttoms = new List<List<KeyboardButton>>();

        var rows = new List<KeyboardButton>()
        {
            new KeyboardButton(Constants.BackText)
        };
        
        buttoms.Add(rows);

        return new ReplyKeyboardMarkup(buttoms) { ResizeKeyboard = true };
    }

    public static ReplyKeyboardMarkup GetUserMenu()
    {
        var buttons = new List<List<KeyboardButton>>();

        var row1 = new List<KeyboardButton>()
        {
            new (Constants.TakeTestText),
            new (Constants.ShowResultText)
        };

        var row2 = new List<KeyboardButton>()
        {
            new (Constants.MessageToAdminText)
        };

        var row3 = new List<KeyboardButton>()
        {
            new (Constants.AboutText),
            new(Constants.AnalyzeTicket)
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
            new (Constants.TakeTestText)
        };

        var row2 = new List<KeyboardButton>()
        {
            new (Constants.ShowResultText),
            new (Constants.GetUsersMessage)
        };

        var row3 = new List<KeyboardButton>()
        {
            new (Constants.ChangeAboutText)
        };
        buttons.Add(row1);
        buttons.Add(row2);
        buttons.Add(row3);

        return  new ReplyKeyboardMarkup(buttons) { ResizeKeyboard = true };
    }
    public static ReplyKeyboardMarkup GetSuperAdminMenu()
    {
        var buttons = new List<List<KeyboardButton>>();

        var row1 = new List<KeyboardButton>()
        {
            new (Constants.TakeTestText),
            new (Constants.ShowResultText),
        };

        var row2 = new List<KeyboardButton>()
        {
           
            new (Constants.GetUsersMessage),
            new (Constants.ChangeAboutText)
        };

        var row3 = new List<KeyboardButton>()
        {
            new(Constants.AddTest),
            new(Constants.DeleteTest)
        };
        var row4 = new List<KeyboardButton>()
        {
            new(Constants.AddAdmin),
            new(Constants.RemoveAdmin)
        };
        var row5 = new List<KeyboardButton>()
        {
            new(Constants.SendTextAds),
            new(Constants.SendFullAds)
        };
        var row6 = new List<KeyboardButton>()
        {
            new(Constants.GetAllUsers),
            new(Constants.AddChannelLink),
            new(Constants.DeleteChenelLink)
        };

        buttons.Add(row1);
        buttons.Add(row2);
        buttons.Add(row3);
        buttons.Add(row4);
        buttons.Add(row5);
        buttons.Add(row6);

        return  new ReplyKeyboardMarkup(buttons) { ResizeKeyboard = true };
    }

    public static ReplyKeyboardMarkup GetChangingInfo()
    {
        var buttons = new List<List<KeyboardButton>>();

        var rows1 = new List<KeyboardButton>()
        {
            new KeyboardButton(Constants.ChangeInfoText),
            new KeyboardButton(Constants.ChangeInfoPhoto)
        };
        var row2 = new List<KeyboardButton>()
        {
            new(Constants.BackText)
        };
        
        buttons.Add(rows1);
        buttons.Add(row2);

        return new ReplyKeyboardMarkup(buttons) { ResizeKeyboard = true };
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

    public static void GetApplications(List<Application> applications)
    {
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("sheet1");

        worksheet.Cells[1, 1].Value = "Numbers";
        worksheet.Cells[1, 2].Value = "FirstName";
        worksheet.Cells[1, 3].Value = "UserName";
        worksheet.Cells[1, 4].Value = "PhoneNumber";
        worksheet.Cells[1, 5].Value = "Role";
        worksheet.Cells[1, 6].Value = "Message";
        worksheet.Cells[1, 7].Value = "CreateDate";

        var row = 2;
        foreach (var application in applications)
        {
            worksheet.Cells[row, 1].Value = row - 1;
            worksheet.Cells[row, 2].Value = application.FirstName;
            worksheet.Cells[row, 3].Value = application.UserName;
            worksheet.Cells[row, 4].Value = application.PhoneNumber;
            worksheet.Cells[row, 5].Value = application.Role;
            worksheet.Cells[row, 6].Value = application.Message;
            worksheet.Cells[row, 7].Value = application.CreateDate.ToString("f");
            row++;
        }
        
        package.SaveAs(new  FileInfo(Constants.ApplicationPath));
    }
    
  public static List<Application> SortApplicationByDate(List<Application> applications,string message)
    {
        var data = message.Split(',').ToArray();
        var fromDateData = data[0].Split('.').ToArray();
        var toDateData = data[1].Split('.').ToArray();


        var fromDate = new DateTime(year:int.Parse(fromDateData[2]),month:int.Parse(fromDateData[1]),day:int.Parse(fromDateData[0]));
        var toDate = new DateTime(year:int.Parse(toDateData[2]),month:int.Parse(toDateData[1]),day:int.Parse(toDateData[0]));
        toDate =  toDate.AddHours(23).AddMinutes(59).AddSeconds(59);

        var sortedApplications = applications
            .Where(x => x.CreateDate >= fromDate && x.CreateDate <= toDate).ToList();

        return applications;
    }
}