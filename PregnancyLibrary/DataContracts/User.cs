using System;
using System.Runtime.Serialization;

namespace PregnancyLibrary.DataContracts
{
    [DataContract]
    public class User
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public DateTime StartTime { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public DateTime LMPDate { get; set; }
    }
}
