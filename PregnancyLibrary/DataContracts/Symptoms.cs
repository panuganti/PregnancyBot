using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PregnancyLibrary.DataContracts
{
    [DataContract]
    public class Symptom
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public SymptomType SymptomType { get; set; }
        [DataMember]
        public string Text { get; set; }
    }

    public enum SymptomType
    {
        Common,
        Recent
    }
}
