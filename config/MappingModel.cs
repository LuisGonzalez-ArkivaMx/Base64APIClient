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
    [JsonConfEditor(NameMember = "ModelName")]
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
        [MFValueListItem(ValueList = "MF.OT.DocumentType")]
        [JsonConfEditor(Label = "Document Type Value")]
        public MFIdentifier DocumentTypeValue { get; set; }
        [DataMember]
        [MFPropertyDef]
        [JsonConfEditor(Label = "Property Document Type")]
        public MFIdentifier PropertyDocumentType { get; set; }
        [DataMember]
        [JsonConfEditor(Label = "Mapeos", ChildName ="Mapeo")]
        public List<MappingProperty> Mappings { get; set; } = new List<MappingProperty>();
        
    }

    [DataContract]
    [JsonConfEditor(NameMember = "FieldName")]
    public class MappingProperty
    {
        [DataMember]
        [JsonConfEditor(Label = "Field Name")]
        public string FieldName { get; set; }

        [DataMember]
        [MFPropertyDef]
        [JsonConfEditor(Label = "Property")]
        public MFIdentifier PropertyName { get; set; }
    }
}
