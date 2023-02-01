# Festive Tech Calendar 2022 - Creating a Holiday Creatures Catalog in Azure with Secure Credentials

In this series of posts and videos, we will look at loading Azure Cosmos DB for NoSQL with some holiday-themed documents. We will create a website to display these documents. And we'll do all of this without storing passwords in code or even in Azure Key Vault - we are going passwordless thanks to the Azure Identity SDK!  So come along for this journey!

![Diagram of the layout accomplished in this series - a console application writes to Azure Cosmos DB with read-write access for an Azure CLI credential. An ASP.NET web application reads the data from Azure Cosmos DB. It uses the Azure CLI credentials when running locally. When in Azure App Service, it uses the Managed Identity credential, which has read-only access to the database. The C# logo and Festive Tech Calendar 2022 logo are included.](architecture.png)

> **Note**: This repository is the companion for the blog series [Creating a Holiday Creatures Catalong in Azure with Secure Credentials](https://www.sadukie.com/2022/12/06/creating-a-holiday-creatures-catalog-in-azure-with-secure-credentials/).

## Prerequisites

You need:

- Azure Cosmos DB for NoSQL account
  - **Note**: The Free tier will work for this demo.
  - Add the resource group name, account name, and URI to the table in [Important Values to Note](#important-values-to-note).
- Azure CLI
- .NET 6

## Important Values to Note

| Field | Your Value | Where Used |
|----|----|----|
|1.  Resource Group Name |&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; | Azure CLI commands|
|2.  Azure Cosmos DB for NoSQL account name |&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; | Azure CLI commands|
|3.  Azure Cosmos DB for NoSQL URI |&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; | Environment Variable: COSMOS_URI|
| 4. "name" from response in read-only role creation (GUID) |   | Used in role assignment |
| 5. "name" from response in read-write role creation (GUID) |   | Used in role assignment |
| 6. Object ID for Azure CLI principal |   | Used in read-write role assignment |
| 7. Object ID for App Service Managed Identity | | Used in read-only role assignment |

## Part 1: Set up custom roles in Azure Cosmos DB for RBAC assignments and grant read-write access

The files we used for this are in the [cosmos-rbac-setup folder](./cosmos-rbac-setup/). In this step, you will create the read-write and read-only roles and assign a read-write role.

> **Note**: This is blogged about here: [Work with Custom Roles for RBAC with Azure Cosmos DB for NoSQL](https://www.sadukie.com/2022/12/06/work-with-custom-roles-for-rbac-with-azure-cosmos-db-for-nosql/)

1. Get the service principal ID for your Azure CLI account. Make note of that ID in the [Important Values to Note](#important-values-to-note).
    1. This can be found by [searching in Azure AD](https://learn.microsoft.com/en-us/azure/marketplace/find-tenant-object-id#find-user-object-id).
    1. Another way is to run this command from Azure CLI and make note of the `id` field: `az ad user show --id user@email.com`
2. Follow [Azure Cosmos DB - How to Setup RBAC](https://learn.microsoft.com/en-us/azure/cosmos-db/how-to-setup-rbac) for more details.
    1. When you create each of the custom roles, pay close attention to the `name` field. Make note of the values in the table above.
3. Grant your Azure CLI account read-write access using the [How to Setup RBAC - Create role assignments](https://learn.microsoft.com/en-us/azure/cosmos-db/how-to-setup-rbac#role-assignments).

## Part 2: Create a .NET Console app to load the data

We are using a .NET console app to load the data.

> **Note**: This is blogged about here: [Create a .NET Console App to Load the Data](https://www.sadukie.com/2022/12/06/create-a-net-console-app-to-load-the-data/)

- We stored our initial properties in a CSV - [HolidayCreatures.csv](./load-data/HolidayCreatures.csv), as it is a common format. We are using the `TextFieldParser` for its handling of CSV files.
- We are using the `ChainedTokenCredential` to connect to Azure Cosmos DB via the Azure CLI credential. This uses the read-write access assigned in Part 1.

(Initially, the Cosmos and Creature files were in a common folder. However, deploying a website with dependencies outside the folder was problematic. So now the files are in each project.)

### .NET Console App Project dependencies

- Microsoft.Azure.Cosmos - version 3
- Azure.Identity - version 1

## Part 3: Create a web application to read the data

We are using an ASP.NET web application to display the records from Azure Cosmos DB for NoSQL.

> **Note**: This is blogged here: [Create a Web Application to Read the Data](https://www.sadukie.com/2022/12/07/create-a-web-application-to-read-the-data/)

- When running the code locally, it will use the Azure CLI credential loaded in the environment where `dotnet run` is called.

- `dotnet new webapp`
- `dotnet run`

### ASP.NET Web App Project dependencies

* Microsoft.Azure.Cosmos - version 3
* Azure.Identity - version 1

## Part 4. Deploy the web application to Azure

Once the web application is running locally and pulling successfully from Azure Cosmos DB without using a password, we can then deploy to an Azure App Service.

> **Note**: This is blogged here: [Get the Passwordless Web Application Running in Azure](https://www.sadukie.com/2022/12/08/get-the-passwordless-web-application-running-in-azure/)

- You will need to enable the Managed Identity on the App Service. When you enable a system-assigned Managed Identity, make note of the object ID in [Important Values to Note](#important-values-to-note).
- You will need to grant **read-only access** to the Managed Identity for Azure Cosmos DB.
- You will also need to add an environment variable called COSMOS_URI.

## Conclusion

In this adventure, we:

- Created read-only and read-write roles to use with Azure Cosmos DB RBAC assignment
- Assigned the read-write role to the Azure CLI principal
- Assigned the read-only role to the Azure App Service Managed Identity
- Used ChainedTokenCredential to handle switching between the credentials for localhost (Azure CLI) and Azure App Service (Managed Identity) without having to expose passwords or write any code to do environment checking 
