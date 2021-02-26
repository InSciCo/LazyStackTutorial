using System;
using System.Collections.Generic;
using System.Text;
using PetStoreSchema.Models;

namespace PetStoreRepo.Models
{

    public interface ICategoryRepo
    {
        public Dictionary<long, Category> Categories { get; }
    }


    public class CategoryRepo : ICategoryRepo
    {
        public CategoryRepo()
        {
            Categories = new Dictionary<long, Category>
            {
                {1, new Category(){Id = 1,Name = "Dog",}},
                {2, new Category(){Id = 2,Name = "Cat",}}
            };
        }

        public Dictionary<long,Category> Categories { get; private set; }

    }

}