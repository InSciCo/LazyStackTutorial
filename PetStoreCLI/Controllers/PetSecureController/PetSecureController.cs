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

namespace PetSecureController
{
    using System = global::System;
    
    [System.CodeDom.Compiler.GeneratedCode("NSwag", "13.10.1.0 (NJsonSchema v10.3.3.0 (Newtonsoft.Json v12.0.0.0))")]
    public interface IPetSecureController
    {
        /// <summary>Add a new pet to the store</summary>
        System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> AddPetAsync(object body);
    
        /// <summary>Update an existing pet</summary>
        System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> UpdatePetAsync(object body);
    
        /// <summary>Updates a pet in the store with form data</summary>
        /// <param name="petId">ID of pet that needs to be updated</param>
        System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> UpdatePetWithFormAsync(long petId, Body body);
    
        /// <summary>Deletes a pet</summary>
        /// <param name="petId">Pet id to delete</param>
        System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> DeletePetAsync(string api_key, long petId);
    
    }
    
    [System.CodeDom.Compiler.GeneratedCode("NSwag", "13.10.1.0 (NJsonSchema v10.3.3.0 (Newtonsoft.Json v12.0.0.0))")]
    public partial class PetSecureController : Microsoft.AspNetCore.Mvc.Controller
    {
        private IPetSecureController _implementation;
    
        public PetSecureController(IPetSecureController implementation)
        {
            _implementation = implementation;
        }
    
        /// <summary>Add a new pet to the store</summary>
        [Microsoft.AspNetCore.Mvc.HttpPost, Microsoft.AspNetCore.Mvc.Route("pet")]
        public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> AddPet([Microsoft.AspNetCore.Mvc.FromBody] object body)
        {
            return _implementation.AddPetAsync(body);
        }
    
        /// <summary>Update an existing pet</summary>
        [Microsoft.AspNetCore.Mvc.HttpPut, Microsoft.AspNetCore.Mvc.Route("pet")]
        public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> UpdatePet([Microsoft.AspNetCore.Mvc.FromBody] object body)
        {
            return _implementation.UpdatePetAsync(body);
        }
    
        /// <summary>Updates a pet in the store with form data</summary>
        /// <param name="petId">ID of pet that needs to be updated</param>
        [Microsoft.AspNetCore.Mvc.HttpPost, Microsoft.AspNetCore.Mvc.Route("pet/{petId}")]
        public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> UpdatePetWithForm(long petId, [Microsoft.AspNetCore.Mvc.FromBody] Body body)
        {
            return _implementation.UpdatePetWithFormAsync(petId, body);
        }
    
        /// <summary>Deletes a pet</summary>
        /// <param name="petId">Pet id to delete</param>
        [Microsoft.AspNetCore.Mvc.HttpDelete, Microsoft.AspNetCore.Mvc.Route("pet/{petId}")]
        public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> DeletePet([Microsoft.AspNetCore.Mvc.FromHeader] string api_key, long petId)
        {
            return _implementation.DeletePetAsync(api_key, petId);
        }
    
    }

}

#pragma warning restore 1591
#pragma warning restore 1573
#pragma warning restore  472
#pragma warning restore  114
#pragma warning restore  108