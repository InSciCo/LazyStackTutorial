using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PetController;
using PetStoreSchema.Models;
using PetStoreRepo.Models;


namespace PetControllerImpl
{
    public class PetControllerImpl : IPetController
    {

        public PetControllerImpl(IPetRepo petRepo)
        {
            this.petRepo = petRepo;
        }

        private IPetRepo petRepo;

        Controller IPetController.LzController { get; set; }
        string IPetController.LzUserId { get; set; }

        public async Task<ActionResult<Pet>> AddPetAsync(Pet body)
        {
            return await AddPetAsync(body);
        }

        public async Task<IActionResult> DeletePetAsync(string api_key, long petId)
        {
            return await petRepo.DeleteAsync(api_key, petId.ToString());
        }

        public async Task<ActionResult<ICollection<Pet>>> FindPetsByStatusAsync(IEnumerable<PetStatus> petStatus)
        {
            return await petRepo.FindPetsByStatusAsync(petStatus);
        }

        public async Task<ActionResult<ICollection<Pet>>> FindPetsByTagsAsync(IEnumerable<string> tags)
        {
            return await petRepo.FindPetsByTagsAsync(tags);
        }

        public async Task<ActionResult<Pet>> GetPetByIdAsync(long petId)
        {
            return await petRepo.GetPetByIdAsync(petId);
        }

        public async Task<ActionResult<ICollection<Category>>> GetPetCategoriesAsync()
        {
            return await petRepo.GetPetCategoriesAsync();
        }

        public async Task<ActionResult<ICollection<Tag>>> GetPetTagsAsync()
        {
            return await petRepo.GetPetTagsAsync();
        }

        public async Task<IActionResult> SeedPetsAsync()
        {
            return await petRepo.SeedPetsAsync();
        }

        public async Task<ActionResult<Pet>> UpdatePetAsync(Pet body)
        {
            return await petRepo.UpdatePetAsync(body);
        }


    }
}
