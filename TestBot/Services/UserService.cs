using Newtonsoft.Json;
using TestBot.Entities;

namespace TestBot.Services;

public class UserService
{
    public  List<User> Users { get; set; }

    private const string Path = "users.json";
    public UserService()
    {
        Users = new List<User>();
        ReadFromFile();
    }

    public User AddUser(long chatId, string? username)
    {
        var user = Users.FirstOrDefault(x => x.ChatId == chatId);
        if (user is null)
        {
            user = new()
            {
                ChatId = chatId,
                UserName = username
            };
            Users.Add(user);
            WriteToFile();
        }
        return user;

    }

   public void UpdateUsser()
    {
        WriteToFile();
    }
    void WriteToFile()
    {
        var jsonData = JsonConvert.SerializeObject(Users);
        File.WriteAllText(Path,jsonData);
        
    }

    void ReadFromFile()
    {
        if (File.Exists(Path))
        {
            var jsonData = File.ReadAllText(Path);
            Users = JsonConvert.DeserializeObject<List<User>>(jsonData)!;
        }
    }
}