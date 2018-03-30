using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;

namespace KotoriCore.Tests.HelperClasses
{
    [DataContract]
    public class MockOpenType : Document
    {
        /// <summary>
        /// PropertyBag to make Edm open-typed
        /// </summary>
        public Dictionary<string, object> PropertyBag { get; set; }
    }
}