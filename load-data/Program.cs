using Microsoft.Azure.Cosmos;
using Microsoft.VisualBasic.FileIO;

namespace Secure_Credentials_For_Advent {
    public class Program {
        private static CosmosHelper cosmosHelper = new();
        private static Container? container;
        static void Main(string[] args){
            cosmosHelper = new();
            container = cosmosHelper.GetContainer().Result;            
            List<Creature> holidayCreatures = GetCreatures();
            int creatureCounter = 1;
            foreach(Creature creature in holidayCreatures){
                creature.CreatureId = $"creature{creatureCounter}";
                container.UpsertItemAsync(creature).Wait();
                creatureCounter++;
            }
        }

        private static List<Creature> GetCreatures(){
            string pathToCsv = "HolidayCreatures.csv";
            List<Creature> holidayCreatures = new List<Creature>();

            using (TextFieldParser parser = new TextFieldParser(pathToCsv)){
                parser.Delimiters = new string[] { ","};
                while(!parser.EndOfData){
                    string[]? fields = parser.ReadFields();
                    if (fields != null){
                        if(fields[0]=="Name") continue;
                        Creature creature = new(name: fields[0], description: fields[1]);
                        creature.IsNaughty = Convert.ToBoolean(fields[2]);
                        holidayCreatures.Add(creature);    
                    }
                }
            }
            return holidayCreatures;
        }
    }
}