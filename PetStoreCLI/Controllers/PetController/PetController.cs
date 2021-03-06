using PetStoreCLISchema.Models;
//----------------------
// <auto-generated>
//     Generated using the NSwag toolchain v13.10.1.0 (NJsonSchema v10.3.3.0 (Newtonsoft.Json v12.0.0.0)) (http://NSwag.org)
// </auto-generated>
//----------------------

#pragma warning disable 108 // Disable "CS0108 '{derivedDto}.ToJson()' hides inherited member '{dtoBase}.ToJson()'. Use the new keyword if hiding was intended."
#pragma warning disable 114 // Disable "CS0114 '{derivedDto}.RaisePropertyChanged(String)' hides inherited member 'dtoBase.RaisePropertyChanged(String)'. To make the current member override that implementation, add the override keyword. Otherwise add the new keyword."
#pragma warning disable 472 // Disable "CS0472 The result of the expression is always 'false' since a value of type 'Int32' is never equal to 'null' of type 'Int32?'
#pragma warning disable 1573 // Disable "CS1573 Parameter '...' has no matching param tag in the XML comment for ...
#pragma warning disable 1591 // Disable "CS1591 Missing XML comment for publicly visible type or member ..."
#pragma warning disable 8073 // Disable "CS8073 The result of the expression is always 'false' since a value of type 'T' is never equal to 'null' of type 'T?'"

namespace PetController
{
    using System = global::System;
    
    [System.CodeDom.Compiler.GeneratedCode("NSwag", "13.10.1.0 (NJsonSchema v10.3.3.0 (Newtonsoft.Json v12.0.0.0))")]
    public interface IPetController
    {
        /// <summary>Finds Pets by status</summary>
        /// <param name="petStatus">Status values that need to be considered for filter</param>
        /// <returns>successful operation</returns>
        System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.ActionResult<System.Collections.Generic.ICollection<Pet>>> FindPetsByStatusAsync(System.Collections.Generic.IEnumerable<PetStatus> petStatus);
    
        /// <summary>Find pet by ID</summary>
        /// <param name="petId">ID of pet to return</param>
        /// <returns>successful operation</returns>
        System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.ActionResult<Pet>> GetPetByIdAsync(long petId);
    
        /// <summary>Get all Pet Categories</summary>
        /// <returns>successful operation</returns>
        System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.ActionResult<System.Collections.Generic.ICollection<Category>>> GetPetCategoriesAsync();
    
        /// <summary>Get all Pet Tags</summary>
        /// <returns>successful operation</returns>
        System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.ActionResult<System.Collections.Generic.ICollection<Tag>>> GetPetTagsAsync();
    
        /// <summary>Finds Pets by tags</summary>
        /// <param name="tags">Tags to filter by</param>
        /// <returns>successful operation</returns>
        [System.Obsolete]
        System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.ActionResult<System.Collections.Generic.ICollection<Pet>>> FindPetsByTagsAsync(System.Collections.Generic.IEnumerable<string> tags);
    
        /// <summary>See pet database</summary>
        /// <returns>successful operation</returns>
        System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> SeedPetsAsync();
    
    }
    
    [System.CodeDom.Compiler.GeneratedCode("NSwag", "13.10.1.0 (NJsonSchema v10.3.3.0 (Newtonsoft.Json v12.0.0.0))")]
    public partial class PetController : Microsoft.AspNetCore.Mvc.Controller
    {
        private IPetController _implementation;
    
        public PetController(IPetController implementation)
        {
            _implementation = implementation;
        }
    
        /// <summary>Finds Pets by status</summary>
        /// <param name="petStatus">Status values that need to be considered for filter</param>
        /// <returns>successful operation</returns>
        [Microsoft.AspNetCore.Mvc.HttpGet, Microsoft.AspNetCore.Mvc.Route("pet/findByStatus")]
        public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.ActionResult<System.Collections.Generic.ICollection<Pet>>> FindPetsByStatus([Microsoft.AspNetCore.Mvc.FromQuery] System.Collections.Generic.IEnumerable<PetStatus> petStatus)
        {
            return _implementation.FindPetsByStatusAsync(petStatus);
        }
    
        /// <summary>Find pet by ID</summary>
        /// <param name="petId">ID of pet to return</param>
        /// <returns>successful operation</returns>
        [Microsoft.AspNetCore.Mvc.HttpGet, Microsoft.AspNetCore.Mvc.Route("pet/{petId}")]
        public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.ActionResult<Pet>> GetPetById(long petId)
        {
            return _implementation.GetPetByIdAsync(petId);
        }
    
        /// <summary>Get all Pet Categories</summary>
        /// <returns>successful operation</returns>
        [Microsoft.AspNetCore.Mvc.HttpGet, Microsoft.AspNetCore.Mvc.Route("pet/categories")]
        public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.ActionResult<System.Collections.Generic.ICollection<Category>>> GetPetCategories()
        {
            return _implementation.GetPetCategoriesAsync();
        }
    
        /// <summary>Get all Pet Tags</summary>
        /// <returns>successful operation</returns>
        [Microsoft.AspNetCore.Mvc.HttpGet, Microsoft.AspNetCore.Mvc.Route("pet/tags")]
        public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.ActionResult<System.Collections.Generic.ICollection<Tag>>> GetPetTags()
        {
            return _implementation.GetPetTagsAsync();
        }
    
        /// <summary>Finds Pets by tags</summary>
        /// <param name="tags">Tags to filter by</param>
        /// <returns>successful operation</returns>
        [System.Obsolete]
        [Microsoft.AspNetCore.Mvc.HttpGet, Microsoft.AspNetCore.Mvc.Route("pet/findByTags")]
        public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.ActionResult<System.Collections.Generic.ICollection<Pet>>> FindPetsByTags([Microsoft.AspNetCore.Mvc.FromQuery] System.Collections.Generic.IEnumerable<string> tags)
        {
            return _implementation.FindPetsByTagsAsync(tags);
        }
    
        /// <summary>See pet database</summary>
        /// <returns>successful operation</returns>
        [Microsoft.AspNetCore.Mvc.HttpGet, Microsoft.AspNetCore.Mvc.Route("pet/seedPets")]
        public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> SeedPets()
        {
            return _implementation.SeedPetsAsync();
        }
    
    }

}

#pragma warning restore 1591
#pragma warning restore 1573
#pragma warning restore  472
#pragma warning restore  114
#pragma warning restore  108