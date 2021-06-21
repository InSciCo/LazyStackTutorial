using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using LazyStackDynamoDBRepo;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using PetStoreSchema.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;



namespace PetStoreRepo.Models
{
    public class PetEnvelope : DataEnvelope<Pet>
    {
        protected override void SetDbRecordFromEntityInstance()
        {
            // Set the Envelope Key fields from the EntityInstance data
            TypeName = "Pet.v1.0.0";
            CreateUtcTick = EntityInstance.CreateUtcTick;
            UpdateUtcTick = EntityInstance.UpdateUtcTick;
            // Primary Key is PartitionKey + SortKey 
            PK = "Pets:"; // Partition key
            SK = $"Pets:{EntityInstance.Id}"; // sort/range key

            // The base method copies information from the envelope keys into the dbRecord
            base.SetDbRecordFromEntityInstance();
        }

        protected override void SetEntityInstanceFromDbRecord()
        {
            base.SetEntityInstanceFromDbRecord();
            EntityInstance.CreateUtcTick = CreateUtcTick;
            EntityInstance.UpdateUtcTick = UpdateUtcTick;
        }

    }

    public interface IPetRepo : IDYDBRepository<PetEnvelope, Pet> 
    {
        Task<ActionResult<Pet>> AddPetAsync(Pet pet);
        Task<IActionResult> DeletePetAsync(string api_key, long petId);
        Task<ActionResult<ICollection<Pet>>> FindPetsByStatusAsync(IEnumerable<PetStatus> status);
        Task<ActionResult<ICollection<Pet>>> FindPetsByTagsAsync(IEnumerable<string> tags);
        Task<ActionResult<Pet>> GetPetByIdAsync(long petId);
        Task<ActionResult<Pet>> UpdatePetAsync(Pet body);
        Task<ActionResult<ICollection<Pet>>> GetInventoryAsync();
        Task<ActionResult<ICollection<Category>>> GetPetCategoriesAsync();
        Task<ActionResult<ICollection<PetStoreSchema.Models.Tag>>> GetPetTagsAsync();
        Task<IActionResult> SeedPetsAsync();

    }

    public class PetRepo : DYDBRepository<PetEnvelope, Pet>, IPetRepo
    {
        public PetRepo(
            IAmazonDynamoDB client,
            ITagRepo tagRepo,
            ICategoryRepo categoryRepo
            ) : base(client, envVarTableName: "TABLE_NAME")
        {
            this.tagRepo = tagRepo;
            this.categoryRepo = categoryRepo;
        }

        private ITagRepo tagRepo;
        private ICategoryRepo categoryRepo;

        // This dictionary allows us to use class attribute names that happen to also be
        // DynamoDB reserved words in our ProjectionExpressions
        Dictionary<string, string> _ExpressionAttributeNames = new Dictionary<string, string>()
            {
                {"#Data", "Data" },
                {"#Status", "Status" },
                {"#General", "General" }
            };

        public async Task<ActionResult<Pet>> AddPetAsync(Pet pet)
        {
            return await CreateAsync(pet);
        }

        public async Task<IActionResult> DeletePetAsync(string api_key, long petId)
        {
            // api_key is ignored in this implementation
            return await DeleteAsync(
                pKPrefix: "Pets:", 
                pKval: string.Empty, 
                sKPrefix: "Pet:", 
                sKval: petId.ToString()); // PK=Pet:, SK=Pet:<petId>
        }

        public async Task<ActionResult<ICollection<Pet>>> FindPetsByStatusAsync(IEnumerable<PetStatus> status)
        {
            var queryRequest = new QueryRequest()
            {
                TableName = tablename,
                KeyConditionExpression = "PK = :PKval AND begins_with(SK, :SKval)",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                        {
                            {":PKval", new AttributeValue() {S = "Pets:"} },
                            {":SKval", new AttributeValue() {S = "Pet:"} }
                        },
                ExpressionAttributeNames = _ExpressionAttributeNames,
                ProjectionExpression = "#Data, TypeName, #Status, UpdateUtcTick, CreateUtcTick, #General"
            };

            var queryResult = await ListAsync(queryRequest);
            var pets = (queryResult.Result as OkObjectResult)?.Value as ICollection<Pet>;
            if (pets!= null)
            {
                var statusList = status.ToList();
                // Filter the list
                var list = new List<Pet>();
                foreach (var pet in pets)
                {
                    if (statusList.Contains(pet.PetStatus))
                        list.Add(pet);
                }
                if (list.Count > 0)
                    return new OkObjectResult(list);
                else
                    return new NoContentResult();
            }
            else
                return queryResult.Result;
        }

        public async Task<ActionResult<ICollection<Pet>>> FindPetsByTagsAsync(IEnumerable<string> tags)
        {
            var queryRequest = new QueryRequest()
            {
                TableName = tablename,
                KeyConditionExpression = "PK = :PKval AND begins_with (SK, :SKval)",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                        {
                            {":PKval", new AttributeValue() {S = "Pets:"} },
                            {":SKval", new AttributeValue() {S = "Pet:"} }
                        },
                ExpressionAttributeNames = _ExpressionAttributeNames,
                ProjectionExpression = "#Data, TypeName, #Status, UpdateUtcTick, CreateUtcTick, #General"
            };

            var queryResult = await ListAsync(queryRequest);
            var pets = (queryResult.Result as OkObjectResult)?.Value as ICollection<Pet>;
            if (pets != null)
            {
                // Filter the list
                var list = new List<Pet>();
                foreach (var pet in pets)
                    foreach (var tag in pet.Tags)
                        if (tags.Contains(tag.Name))
                            list.Add(pet);
                if (list.Count > 0)
                    return new OkObjectResult(list);
                else
                    return new NoContentResult();
            }
            else
                return queryResult.Result;
        }

        public async Task<ActionResult<Pet>> GetPetByIdAsync(long petId)
        {
            return await ReadAsync(
                pKPrefix: $"Pets:", 
                pKval: string.Empty, 
                sKPrefix: "Pet:", 
                sKval: petId.ToString());
        }

        public async Task<ActionResult<Pet>> UpdatePetAsync(Pet body)
        {
            return await UpdateAsync(body);
        }

        public async Task<ActionResult<ICollection<Pet>>> GetInventoryAsync()
        {
            var queryRequest = new QueryRequest()
            {
                TableName = tablename,
                KeyConditionExpression = "PK = :PKval AND begins_with (SK, :SKval)",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                        {
                            {":PKval", new AttributeValue() {S = "Pets:"} },
                            {":SKval",new AttributeValue() {S = "Pet:"} }
                        },
                ExpressionAttributeNames = _ExpressionAttributeNames,
                ProjectionExpression = "#Data, TypeName, #Status, UpdateUtcTick, CreateUtcTick, #General"
            };

            return await ListAsync(queryRequest);
        }

        public async Task<ActionResult<Pet>> PingAsync()
        {
            await Task.Delay(0);
            return new OkObjectResult(
                new Pet()
                {
                    Id = 10,
                    Category = categoryRepo.Categories[1],
                    Name = "pingpet",
                    PetStatus = PetStatus.Available,
                    Tags = new List<PetStoreSchema.Models.Tag> { tagRepo.Tags[1] },
                    PhotoUrls = new List<string> { "https://www.example.com" }
                });
        }

        public async Task<ActionResult<ICollection<Category>>> GetPetCategoriesAsync()
        {
            await Task.Delay(0);
            return categoryRepo.Categories.Values.ToList();
        }

        public  async Task<ActionResult<ICollection<PetStoreSchema.Models.Tag>>> GetPetTagsAsync()
        {
            await Task.Delay(0);
            return tagRepo.Tags.Values.ToList();
        }

        public async Task<IActionResult> SeedPetsAsync()
        {

            var pet = new Pet()
            {
                Id = 1,
                Category = categoryRepo.Categories[1], // dog
                Name = "Alf",
                PhotoUrls = new List<string> { "www.example.com" },
                Tags = new List<PetStoreSchema.Models.Tag> { tagRepo.Tags[1], tagRepo.Tags[2] },
                PetStatus = PetStatus.Available
            };


            await AddPetAsync(pet);

            pet = new Pet()
            {
                Id = 2,
                Category = categoryRepo.Categories[2], // cat
                Name = "Sweetie",
                PhotoUrls = new List<string> { "www.example.com" },
                Tags = new List<PetStoreSchema.Models.Tag> { tagRepo.Tags[1], tagRepo.Tags[2], tagRepo.Tags[3] },
                PetStatus = PetStatus.Available
            };

            await AddPetAsync(pet);

            return new OkResult();
        }
    }
}
