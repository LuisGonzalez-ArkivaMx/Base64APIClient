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
    public class MappingModel
    {
        [DataMember]
        [JsonConfEditor(Label = "Model Name")]
        public string ModelName { get; set; }
        [DataMember]
        [MFObjType]
        [JsonConfEditor(Label = "Object Type")]
        public MFIdentifier ObjectType { get; set; }
        [DataMember]
        [MFClass]
        [JsonConfEditor(Label = "Class")]
        public MFIdentifier Type { get; set; }
        [DataMember]
        [JsonConfEditor(Label = "Mapeos")]
        public List<MappingProperty> Mappings { get; set; } = new List<MappingProperty>();
        
    }

    [DataContract]
    public class MappingProperty
    {
        [DataMember]
        [JsonConfEditor(Label = "Field Name")]
        public string FieldName { get; set; }

        [DataMember]
        [MFPropertyDef(Required = true)]
        [JsonConfEditor(Label = "Property")]
        public MFIdentifier PropertyName { get; set; }
    }
}
