namespace TestBot.Entities;

public class TicketInfo
{
    public  int  NextTestId { get; set; }
    public  int EndTo { get; set; }

    public bool IsCompleted => EndTo  > NextTestId;
}