using System;
using System.Collections.Generic;
using System.Text;
using PetStoreSchema.Models;

namespace PetStoreRepo.Models
{

    public interface ITagRepo
    {
        public Dictionary<long, Tag> Tags { get; }
    }

    public class TagRepo : ITagRepo
    {
        public TagRepo()
        {
            Tags = new Dictionary<long, Tag>
            {
                {1, new Tag() {Id=1, Name="HasShots"} },
                {2, new Tag() {Id=2, Name="HouseTrained"} },
                {3, new Tag() {Id=3, Name="Crazy"} }
            };
        }
        public Dictionary<long, Tag> Tags { get; private set; }

    }
}
