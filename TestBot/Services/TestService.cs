using System.Net.Http.Json;
using Newtonsoft.Json;
using TestBot.Entities;

namespace TestBot.Services;

public class TestService
{
    public  List<Test> Tests { get; set; }

    public TestService()
    {
        Tests = new List<Test>();
        ReadTests();
    }

    private void ReadTests()
    {
        var jsonData = File.ReadAllText("uzlotin.json");
        Tests = JsonConvert.DeserializeObject<List<Test>>(jsonData)!;
    }
}