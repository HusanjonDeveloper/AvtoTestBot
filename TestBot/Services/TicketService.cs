using Newtonsoft.Json;
using TestBot.Entities;

namespace TestBot.Services;

public class TicketService
{
    public List<Ticket> Tickets { get; set; }
    private readonly string path = "tickets.json";

    public TicketService()
    {
        Tickets = new();
        ReadToFile();
    }

    public Ticket AddOrUpdate(long chatId, byte ticketId)
    {
        var ticket = Tickets.FirstOrDefault(x => x.Id == ticketId && x.ChatId == chatId);

        if (ticket is null)
        {
            ticket = new Ticket()
            {
                Id = ticketId,
                ChatId = chatId
            };
            Tickets.Add(ticket);
            UpdateTicket();
        }

        return ticket;
    }

    public void UpdateTicket()
    {
        WriteToFile();
    }

    void WriteToFile()
    {
        var jsonData = JsonConvert.SerializeObject(Tickets);
        File.WriteAllText(path,jsonData);
    }

    void ReadToFile()
    {
        if (File.Exists(path))
        {
            var jsonData = File.ReadAllText(path);
            Tickets = JsonConvert.DeserializeObject<List<Ticket>>(path)!;
        }
    }
}