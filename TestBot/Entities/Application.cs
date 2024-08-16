namespace TestBot.Entities;

public class Application
{
    public  string Message { get; set; }
    
    public  long ChatId { get; set; }
    
    public  string FirstName { get; set; }
    
    public  string? UserName { get; set; }

    public  string? PhoneNumber { get; set; }
    
    public  string Role { get; set; }

    public DateTime CreateDate { get; set; } = DateTime.Now;
}