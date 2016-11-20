using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PregnancyLibrary.DataContracts
{
    [DataContract]
    public class BotToUserMilestones
    {
        [DataMember]
        public BotToUserMilestonesTypes Type { get; set; }
        [DataMember]
        public string UserId { get; set; }
        [DataMember]
        public DateTime DateMilestoneRecorded { get; set; }
    }

    [DataContract]
    public enum BotToUserMilestonesTypes
    {
        [EnumMember]
        LMP,
        [EnumMember]
        Introduction
    }
}
