using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using LazyStackDynamoDBRepo;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using PetStoreSchema.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OrderController;
using OrderSecureController;

namespace PetStoreRepo.Models
{
    public class OrderHelper : IEntityHelper<Order, DefaultEnvelope>
    {
        public Order Instance { get; set; }

        public void SetCreateUtcTick(long t) { Instance.CreateUtcTick = t; }
        public void SetUpdateUtcTick(long t) { Instance.UpdateUtcTick = t; }
        public long GetUpdateUtcTick() { return Instance.UpdateUtcTick; }

        // Update the keys and data in the envelope from the current Instance
        public void UpdateEnvelope(DefaultEnvelope env, DefaultEnvelope dbEnvelope = null, bool serialize = false)
        {
            env.TypeName = nameof(Order);
            env.CreateUtcTick = Instance.CreateUtcTick;
            env.UpdateUtcTick = Instance.UpdateUtcTick;

            // Primary Key is PartionKey + SortKey
            env.PK = "Orders:"; // partition key
            env.SK = $"Order:{Instance.Id}"; // sort/range key

            if (serialize)
                env.Data = JsonConvert.SerializeObject(Instance);
        }

        // Return an object instance of Instance
        public Order Deserialize(DefaultEnvelope env)
        {
            return JsonConvert.DeserializeObject<Order>(env.Data);
        }
    }

    public interface IOrderRepo : IOrderController, IOrderSecureController { }

    public class OrderRepo : DYDBRepository<DefaultEnvelope, OrderHelper, Order>,  IOrderRepo
    {
        public OrderRepo(
            IAmazonDynamoDB client,
            IPetRepo petRepo
            ) : base(client, envVarTableName: "TABLE_NAME")
        {
            this.petRepo = petRepo;
        }

        IPetRepo petRepo;

        // This dictionary allows us to use class attribute names that happen to also be
        // DynamoDB reserved words in our ProjectionExpressions
        Dictionary<string, string> _ExpressionAttributeNames = new Dictionary<string, string>()
        {
            {"#Data", "Data" },
            {"#Status", "Status" },
            {"#General", "General" }
        };

        public async Task<ActionResult<Order>> PlaceOrderAsync(Order body)
        {
            return await CreateAsync(body as Order) as ActionResult;
        }

        public async Task<ActionResult<Order>> GetOrderByIdAsync(long orderId)
        {
            return await ReadAsync(
                 pKPrefix: "Orders:",
                 pKval: string.Empty,
                 sKPrefix: "Order:",
                 sKval: orderId.ToString()) as ActionResult;
        }

        public async Task<IActionResult> DeleteOrderAsync(long orderId)
        {
            return await DeleteAsync(
                pKPrefix: "Orders:",
                pKval: string.Empty,
                sKPrefix: "Order:",
                sKval: orderId.ToString());
        }

        public async Task<ActionResult<IDictionary<string, int>>> GetInventoryAsync()
        {
            var allPetsResult = await petRepo.GetInventoryAsync();
            if (allPetsResult.GetType() == typeof(OkObjectResult))
            {
                var value = (allPetsResult as OkObjectResult).Value as ICollection<Pet>;
                if (value == null)
                    return new NoContentResult();

                var inventory = new Dictionary<string, int>();
                foreach (var pet in value)
                {
                    var status = pet.PetStatus.ToString();
                    if (!inventory.ContainsKey(status))
                        inventory.Add(status, 0);
                    inventory[status] += 1;
                }
                return new OkObjectResult(inventory);
            }
            else
                return allPetsResult as ActionResult;
        }

    }
}
