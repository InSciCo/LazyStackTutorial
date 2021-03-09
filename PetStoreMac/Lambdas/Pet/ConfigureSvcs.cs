// Generated by LazyStack - modifications will be overwritten
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace LambdaFunc
{
    public partial class Startup
    {
        public void ConfigureSvcs(IServiceCollection services)
        {
            services.AddSingleton<PetStoreRepo.Models.PetRepo>();
            services.AddSingleton<PetStoreRepo.Models.IPetRepo>(x => x.GetRequiredService<PetStoreRepo.Models.PetRepo>());
            services.AddSingleton<PetController.IPetController>(x => x.GetRequiredService<PetStoreRepo.Models.PetRepo>());
            services.AddSingleton<PetSecureController.IPetSecureController>(x => x.GetRequiredService<PetStoreRepo.Models.PetRepo>());
            services.AddSingleton<PetStoreRepo.Models.OrderRepo>();
            services.AddSingleton<OrderController.IOrderController>(x => x.GetRequiredService<PetStoreRepo.Models.OrderRepo>());
            services.AddSingleton<OrderSecureController.IOrderSecureController>(x=> x.GetRequiredService<PetStoreRepo.Models.OrderRepo>());
            services.AddSingleton<PetStoreRepo.Models.ITagRepo,PetStoreRepo.Models.TagRepo>();
            services.AddSingleton<PetStoreRepo.Models.ICategoryRepo,PetStoreRepo.Models.CategoryRepo>();
            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
            services.AddAWSService<Amazon.DynamoDBv2.IAmazonDynamoDB>();
        }
    }
}