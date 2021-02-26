# PetStoreCLI Setup
To run the projects in this solution you will need to install 
these <a href="https://lazystack.io/installation/installation_netcorecli.html">prerequisites</a>.

Then follow these steps:
1. Open the LazyStacks.yaml file and edit the Region and Profile name in the Stacks.Dev element.
2. In the solution folder, use the lazystack CLI tool to regenerate projects:
    - ``lazystack projects``
3. Then rebuild the projects
    - ``dotnet build``
4. Create an S3 bucket
    - ``aws s3api create-bucket --bucket petstorecli --region us-east-1 --acl private``
5. Publish stack
    - ``sam deploy -t .\Stacks\Dev\serverless.template --stack-name PetStoreCLIDev --s3-bucket petstorecli --capabilities  CAPABILITY_NAMED_IAM``
6. Generate AwsSettings.json file
    - ``lazystack settings PetStoreCLIDev .\Stacks\Dev\AwsSettings.json``
7. Build the solution
    - ``dotnet build``
8. You may now run the PetStoreConsoleApp
    - ``dotnet run -p PetStoreConsoleApp``
    
For more detailed review of this project, visit the <a href="https://lazystack.io/tutorial/tutorial_cli_overview.html">LazyStack DotNet CLI Tutorial</a>.

