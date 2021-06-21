using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OrderController;
using PetStoreSchema.Models;
using PetStoreRepo.Models;

namespace OrderControllerImpl
{
    public class OrderControllerImpl : IOrderController
    {

        public OrderControllerImpl(IOrderRepo orderRepo, IPetRepo petRepo)
        {
            this.petRepo = petRepo;
            this.orderRepo = orderRepo;
        }

        public Controller LzController { get; set; }
        public string LzUserId { get; set; }

        private IPetRepo petRepo;
        private IOrderRepo orderRepo;

        public async Task<IActionResult> DeleteOrderAsync(long orderId)
        {
           return await orderRepo.DeleteOrderAsync(orderId);
        }

        public async Task<ActionResult<IDictionary<string, int>>> GetInventoryAsync()
        {
            var result = await petRepo.GetInventoryAsync();
            var pets = (result.Result as OkObjectResult)?.Value as ICollection<Pet>;
            if (pets != null)
            {
                var inventory = new Dictionary<string, int>();
                foreach (var pet in pets)
                {
                    var status = pet.PetStatus.ToString();
                    if (!inventory.ContainsKey(status))
                        inventory.Add(status, 0);
                    inventory[status] += 1;
                }
                if (inventory.Count > 0)
                    return new OkObjectResult(inventory);
                else
                    return new NoContentResult();
            }
            else
                return result.Result;
        }

        public async Task<ActionResult<Order>> GetOrderByIdAsync(long orderId)
        {
            return await orderRepo.GetOrderByIdAsync(orderId);
        }

        public async Task<ActionResult<Order>> PlaceOrderAsync(Order body)
        {
            return await orderRepo.PlaceOrderAsync(body);
        }
    }
}
