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


namespace PetStoreRepo.Models
{
    public class OrderEnvelope : DataEnvelope<Order>
    {

        protected override void SetDbRecordFromEntityInstance()
        {
            // Set the Envelope Key fields from the EntityInstance data
            TypeName = "Order.v1.0.0";
            CreateUtcTick = EntityInstance.CreateUtcTick;
            UpdateUtcTick = EntityInstance.UpdateUtcTick;
            // Primary Key is PartitionKey + SortKey 
            PK = "Orders:"; // Partition key
            SK = $"Order:{EntityInstance.Id}"; // sort/range key

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

    public interface IOrderRepo : IDYDBRepository<OrderEnvelope, Order> 
    {
        Task<ActionResult<Order>> PlaceOrderAsync(Order body);
        Task<ActionResult<Order>> GetOrderByIdAsync(long orderId);
        Task<IActionResult> DeleteOrderAsync(long orderId);
    }

    public class OrderRepo : DYDBRepository<OrderEnvelope, Order>, IOrderRepo
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
            return await CreateAsync(body as Order);
        }

        public async Task<ActionResult<Order>> GetOrderByIdAsync(long orderId)
        {
            return await ReadAsync(
                 pKPrefix: "Orders:",
                 pKval: string.Empty,
                 sKPrefix: "Order:",
                 sKval: orderId.ToString());
        }

        public async Task<IActionResult> DeleteOrderAsync(long orderId)
        {
            return await DeleteAsync(
                pKPrefix: "Orders:",
                pKval: string.Empty,
                sKPrefix: "Order:",
                sKval: orderId.ToString());
        }
    }
}

