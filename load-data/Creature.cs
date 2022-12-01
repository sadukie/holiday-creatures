using Newtonsoft.Json;

namespace Secure_Credentials_For_Advent;
public class Creature {
    [JsonProperty("id")]
    public string CreatureId {get;set; } = Guid.NewGuid().ToString();
    public string Name {get; set;} = "";
    public string Description {get;set;} = "";
    public bool IsNaughty { get;set; } = false;
    public Creature() {

    }

    public Creature(string name, string description){
        Name = name;
        Description = description;
    }
}
