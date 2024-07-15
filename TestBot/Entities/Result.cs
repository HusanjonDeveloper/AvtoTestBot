namespace TestBot.Entities;

public class Result
{
    public byte TotalAnswerCount = 20;
    public byte CorrecAnswerCount { get; set; }
    public byte InCorrectAnswerCount => (byte)(TotalAnswerCount - CorrecAnswerCount);
}