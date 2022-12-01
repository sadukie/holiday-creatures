using Microsoft.Azure.Cosmos;

namespace Secure_Credentials_For_Advent;

public class CreatureHelper{

    public CreatureHelper(){}

    public async static Task<IEnumerable<Creature>> RetrieveAllCreaturesAsync(){
        CosmosHelper cosmosHelper = new();
        Container container = cosmosHelper.GetContainer().Result;
        List<Creature> creatures = new();
        using FeedIterator<Creature> feed = container.GetItemQueryIterator<Creature>(
            queryText: "SELECT * FROM Creatures"
        );
        while (feed.HasMoreResults)
        {
            FeedResponse<Creature> response = await feed.ReadNextAsync();

            // Iterate query results
            foreach (Creature creature in response)
            {
                creatures.Add(creature);
            }
        }
        return creatures;
    }
}