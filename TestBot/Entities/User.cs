namespace TestBot.Entities;

public class User
{
    public  long ChatId { get; set; }
    public  string FirstName { get; set; }
    public  string? UserName { get; set; }
    public Step UserStep { get; set; }
    public  string? PhoneNumber { get; set; }
    public TicketInfo? TicketInfo { get; set; }

}