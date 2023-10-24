using MFiles.VAF.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Base64ApiClient.config
{
    [DataContract]
    [JsonConfEditor()]
    public class SearchConditionDocument
    {
        [MFObject]
        [DataMember]
        [JsonConfEditor(HelpText = "Object Type", Label = "Object Type")]
        public MFIdentifier ObjectType { get; set; }

        [MFClass]
        [DataMember]
        [JsonConfEditor(HelpText = "Clase", Label = "Clase")]
        public MFIdentifier Class { get; set; }
    }
}
