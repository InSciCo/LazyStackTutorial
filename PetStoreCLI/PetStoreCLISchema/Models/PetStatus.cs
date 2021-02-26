
//----------------------
// <auto-generated>
//     Generated using the NSwag toolchain 
// </auto-generated>
// Refactored into the schema project by LazyStack
//----------------------

#pragma warning disable 108 // Disable "CS0108 '{derivedDto}.ToJson()' hides inherited member '{dtoBase}.ToJson()'.Use the new keyword if hiding was intended."
#pragma warning disable 114 // Disable "CS0114 '{derivedDto}.RaisePropertyChanged(String)' hides inherited member 'dtoBase.RaisePropertyChanged(String)'. To make the current member override that implementation, add the override keyword. Otherwise add the new keyword."
#pragma warning disable 472 // Disable "CS0472 The result of the expression is always 'false' since a value of type 'Int32' is never equal to 'null' of type 'Int32?'"
#pragma warning disable 1573 // Disable "CS1573 Parameter '...' has no matching param tag in the XML comment for ..."
#pragma warning disable 1591 // Disable "CS1591 Missing XML comment for publicly visible type or member ..."
#pragma warning disable 8073 // Disable "CS8073 The result of the expression is always 'false' since a value of type 'T' is never equal to 'null' of type 'T?'"

namespace PetStoreCLISchema.Models
{
    using System = global::System;

     
    /// <summary>pet status in the store</summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.3.3.0 (Newtonsoft.Json v12.0.0.0)")]
    public enum PetStatus
    {
        [System.Runtime.Serialization.EnumMember(Value = @"available")]
        Available = 0,
    
        [System.Runtime.Serialization.EnumMember(Value = @"pending")]
        Pending = 1,
    
        [System.Runtime.Serialization.EnumMember(Value = @"sold")]
        Sold = 2,
    
    }

}
#pragma warning restore 1591
#pragma warning restore 1573
#pragma warning restore  472
#pragma warning restore  114
#pragma warning restore  108