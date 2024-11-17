## Instructions

1. Make sure you have completed the instructions for setting up the checkout as you will need API keys etc for this.

2. To build from source, install the sdk: https://dotnet.microsoft.com/en-us/download/dotnet/8.0. Or download one of the releases and go to straight the configuring section.

3. I have added the Monero daemon and wallet rpc to the application for ease of use but if you want to download and verify them yourself that works too.

4. There are existing publish profiles in Properties/PublishProfiles - cd into Monero.Api.
	1. For win-x64 run: dotnet publish Monero.Api.csproj /p:PublishProfile=Properties/PublishProfiles/win64prod.pubxml
	2. For linux-x64 run: dotnet publish Monero.Api.csproj /p:PublishProfile=Properties/PublishProfiles/linux64prod.pubxml

Profiles for 32bit, arm and mac can also be created.
The program can then be found in bin/Release/net8.0/[architecture]/publish. You can move the publish folder if you want.

5. The wallet rpc and daemon can be found in the Programs folder, if you want to use your own you can just replace them.

6. We need to build the Fetch program which is used to notify the api of incoming transactions. From the root of the repo cd into the Fetch directory.
	1. For win-x64 run: dotnet publish Fetch.csproj /p:PublishProfile=Properties/PublishProfiles/win64prod.pubxml
	2. For linux-x64 run: dotnet publish Fetch.csproj /p:PublishProfile=Properties/PublishProfiles/linux64prod.pubxml

## Configuring

1. In the publish folder, open the appsettings.json file. 
	1. Change the network to mainnet, and if using a local node, set the data directory to where the blockchain is stored on your computer.
	2. Set the number of confirmations, (supports 0-conf) - the default is 10.
	3. Change the first API key to something else, use a password generator or similar.
	4. WalletRPCPassword is not used at the moment as it runs on the same machine and should not be port forwarded.
	5. For BigCommerceSettings, set the APIKey and StoreHash/store id to the ones you got from the checkout setup and then save.

2. Create a file (or use the one in the project) called moneroapiurl.json with the content below and change the address to the address of where the monero api will be running - use a vps or similar to host the api, this is so that the checkout can call the api.
And then upload it to the content folder in the same way we uploaded the checkout folder earlier.

{
  "url": "https://[IP ADDRESS OF MONERO API SERVER]:5100/api"
}

![alt text](https://i.ibb.co/cFTn94V/14.png)

3. You can now run the Monero.Api program. You will need to forward port 5100.