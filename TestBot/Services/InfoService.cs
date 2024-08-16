using Newtonsoft.Json;
using TestBot.Entities;

namespace TestBot.Services;

public class InfoService
{
    private const string Path = "info.json";
    public Info Info { get; set; }

    public InfoService()
    {
        Info = new Info();
        ReadFromFile();
    }

    public void ChangeInfoText(string text)
    {
        Info.Text = text;
        WriteToFile();
    }
    
    public void ChangeInfoPhotoUrl(string url)
    {
        Info.PhotoUrl = url;
        WriteToFile();
    }
    void WriteToFile()
    {
        var jsonData = JsonConvert.SerializeObject(Path);
        File.WriteAllText(Path,jsonData);
    }

    void ReadFromFile()
    {
        if (File.Exists(Path))
        {
            var jsonData = File.ReadAllText(Path);
            Info = JsonConvert.DeserializeObject<Info>(jsonData)!;   
        }
    }
}