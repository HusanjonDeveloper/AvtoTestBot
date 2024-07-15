namespace TestBot.Entities;

public class Ticket
{
    public  int Id { get; set; }
    public  long ChatId { get; set; }
    public  Result? Result  { get; set; }
    public  DateTime TookAt { get; set; } = DateTime.Now;
}