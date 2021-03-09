using PetStoreMacSchema.Models;
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

namespace OrderController
{
    using System = global::System;
    
    [System.CodeDom.Compiler.GeneratedCode("NSwag", "13.10.1.0 (NJsonSchema v10.3.3.0 (Newtonsoft.Json v12.0.0.0))")]
    public interface IOrderController
    {
        /// <summary>Place an order for a pet</summary>
        /// <param name="body">order placed for purchasing the pet</param>
        /// <returns>successful operation</returns>
        System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.ActionResult<Order>> PlaceOrderAsync(Order body);
    
        /// <summary>Find purchase order by ID</summary>
        /// <param name="orderId">ID of pet that needs to be fetched</param>
        /// <returns>successful operation</returns>
        System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.ActionResult<Order>> GetOrderByIdAsync(long orderId);
    
    }
    
    [System.CodeDom.Compiler.GeneratedCode("NSwag", "13.10.1.0 (NJsonSchema v10.3.3.0 (Newtonsoft.Json v12.0.0.0))")]
    public partial class OrderController : Microsoft.AspNetCore.Mvc.Controller
    {
        private IOrderController _implementation;
    
        public OrderController(IOrderController implementation)
        {
            _implementation = implementation;
        }
    
        /// <summary>Place an order for a pet</summary>
        /// <param name="body">order placed for purchasing the pet</param>
        /// <returns>successful operation</returns>
        [Microsoft.AspNetCore.Mvc.HttpPost, Microsoft.AspNetCore.Mvc.Route("order")]
        public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.ActionResult<Order>> PlaceOrder([Microsoft.AspNetCore.Mvc.FromBody] Order body)
        {
            return _implementation.PlaceOrderAsync(body);
        }
    
        /// <summary>Find purchase order by ID</summary>
        /// <param name="orderId">ID of pet that needs to be fetched</param>
        /// <returns>successful operation</returns>
        [Microsoft.AspNetCore.Mvc.HttpGet, Microsoft.AspNetCore.Mvc.Route("order/{orderId}")]
        public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.ActionResult<Order>> GetOrderById(long orderId)
        {
            return _implementation.GetOrderByIdAsync(orderId);
        }
    
    }

}

#pragma warning restore 1591
#pragma warning restore 1573
#pragma warning restore  472
#pragma warning restore  114
#pragma warning restore  108