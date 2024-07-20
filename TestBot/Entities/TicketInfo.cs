namespace TestBot.Entities;

public class TicketInfo
{
    public  byte TicketId { get; set; }
    public  int  NextTestId { get; set; }
    public  int EndTo { get; set; }

    public bool IsCompleted => EndTo  > NextTestId;
}