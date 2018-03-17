using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;

namespace KotoriCore.Tests
{
    [DataContract]
    public enum MockEnum
    {
        [EnumMember]
        ZERO,

        [EnumMember]
        ONE,

        [EnumMember]
        TWO
    }


    [DataContract]
    public class MockOpenType : Document
    {
        /// <summary>
        /// The property name for EnglishName
        /// </summary>
        public const string EnglishNamePropertyName = "englishName";

        /// <summary>
        /// The property name for EnumNumber
        /// </summary>
        public const string EnumNumberPropertyName = "enumNumber"; 

        /// <summary>
        /// EnglishName
        /// </summary>
        [DataMember(Name = EnglishNamePropertyName)]
        [JsonProperty(PropertyName = EnglishNamePropertyName)]
        public string EnglishName { get; set; }

        /// <summary>
        /// EnumNumber
        /// </summary>
        [DataMember(Name = EnumNumberPropertyName)]
        [JsonProperty(PropertyName = EnumNumberPropertyName)]
        public MockEnum EnumNumber { get; set; }

        /// <summary>
        /// PropertyBag to make Edm open-typed
        /// </summary>
        public Dictionary<string, object> PropertyBag { get; set; }
    }
}