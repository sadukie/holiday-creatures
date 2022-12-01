using Azure.Identity;
using Microsoft.Azure.Cosmos;

namespace Secure_Credentials_For_Advent;

public class CosmosHelper
{
    CosmosClient client;

    private static string CosmosUri => Environment.GetEnvironmentVariable("COSMOS_URI") ?? throw new ArgumentException("Missing env var: COSMOS_URI");
    private static string DatabaseName = "HolidayCreatures";
    private static string ContainerName = "Creatures";
    private static string PartitionKey = "/id";

    public CosmosHelper()
    {
        ChainedTokenCredential credential = new ChainedTokenCredential(new AzureCliCredential(),new ManagedIdentityCredential());

        client = new(
            accountEndpoint: CosmosUri,
            tokenCredential: credential);
    }

    async public Task<Database> GetDatabase()
    {
        Database database = await client.CreateDatabaseIfNotExistsAsync(
            id: DatabaseName
        );

        return database;
    }

    async public Task<Container> GetContainer()
    {
        Database database = await GetDatabase();

        Container container = await database.CreateContainerIfNotExistsAsync(
            id: ContainerName,
            partitionKeyPath: PartitionKey,
            throughput: 400
        );

        return container;
    }        
}
