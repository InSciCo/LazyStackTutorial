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

using PetController;
using PetSecureController;

namespace PetStoreRepo.Models
{
    public class PetHelper : IEntityHelper<Pet, DefaultEnvelope>
    {
        public Pet Instance { get; set; }

        public void SetCreateUtcTick(long t) { Instance.CreateUtcTick = t; }
        public void SetUpdateUtcTick(long t) { Instance.UpdateUtcTick = t; }
        public long GetUpdateUtcTick() { return Instance.UpdateUtcTick; }

        // Update the keys and data in the envelope from the current Instance
        public void UpdateEnvelope(DefaultEnvelope env, DefaultEnvelope dbEnvelope = null, bool serialize = false)
        {
            env.TypeName = nameof(Pet);
            env.CreateUtcTick = Instance.CreateUtcTick;
            env.UpdateUtcTick = Instance.UpdateUtcTick;

            // Primary Key is partion + sortkey
            env.PK = "Pets:"; // partition
            env.SK = $"Pet:{Instance.Id}"; // sortkey

            if (serialize)
                env.Data = JsonConvert.SerializeObject(Instance);
        }

        // Return an object instance of Instance
        public Pet Deserialize(DefaultEnvelope env)
        {
            return JsonConvert.DeserializeObject<Pet>(env.Data);
        }
    }

    public interface IPetRepo : IPetController, IPetSecureController 
    {
        Task<IActionResult> GetInventoryAsync();
    }

    public class PetRepo : DYDBRepository<DefaultEnvelope, PetHelper, Pet>, IPetRepo
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

        public async Task<IActionResult> AddPetAsync(object body)
        {
            var data = body as Pet;
            if(data == null)
                return new BadRequestObjectResult("AddPet passed invalid pet object");

            return await CreateAsync(data);
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
            if (queryResult.GetType() == typeof(OkObjectResult))
            {
                var statusList = status.ToList();
                // Filter the list
                var list = new List<Pet>();
                foreach (var pet in (queryResult as OkObjectResult).Value as List<Pet>)
                {
                    if (statusList.Contains(pet.PetStatus))
                        list.Add(pet);
                }
                return new OkObjectResult(list);
            }
            else
                return queryResult as ActionResult;
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
            if (queryResult.GetType() == typeof(OkObjectResult))
            {
                // Filter the list
                var list = new List<Pet>();
                foreach (var pet in (queryResult as OkObjectResult).Value as List<Pet>)
                    foreach (var tag in pet.Tags)
                        if (tags.Contains(tag.ToString()))
                            list.Add(pet);

                return new OkObjectResult(list);
            }
            else
                return queryResult as ActionResult;
        }

        public async Task<ActionResult<Pet>> GetPetByIdAsync(long petId)
        {
            return await ReadAsync(
                pKPrefix: $"Pets:", 
                pKval: string.Empty, 
                sKPrefix: "Pet:", 
                sKval: petId.ToString()) as ActionResult;
        }

        public async Task<IActionResult> UpdatePetAsync(object body)
        {
            return await UpdateAsync(body as Pet);
        }

        public async Task<IActionResult> UpdatePetWithFormAsync(long petId, Body body)
        {
            var current = await ReadAsync(
                pKPrefix: "Pets:", 
                pKval: string.Empty,
                sKPrefix: "Pet:",
                sKval: petId.ToString());

            if(current.GetType() != typeof(OkObjectResult))
                return current;

            var pet = (current as OkObjectResult).Value as Pet;
            pet.Name = body.Name;
            pet.PetStatus = (PetStatus)Enum.Parse(typeof(PetStatus), body.Status, false);
            return await UpdateAsync(pet);
        }

        public async Task<IActionResult> GetInventoryAsync()
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
