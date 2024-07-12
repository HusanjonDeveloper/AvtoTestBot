namespace TestBot.Entities;

public class Test
{
    public  int Id { get; set; }
    public  string Question { get; set; }
    public  List<Choice>  Choices{ get; set; }
    public  Media Media { get; set; }
    public  string Description { get; set; }
}