﻿using MFiles.VAF.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Base64ApiClient.config
{
    [DataContract]
    [MFWorkflow(RefMember = "Workflow")]
    public class UnclassifiedModel
    {
        [DataMember]
        [MFPropertyDef]
        [JsonConfEditor(Label = "Property Document Type(Unclassified)")]
        public MFIdentifier UnclassifiedPropertyDocumentType { get; set; }
        [DataMember]
        [MFValueListItem(ValueList = "MF.OT.DocumentType")]
        [JsonConfEditor(Label = "Document Type Value(Unclassified)")]
        public MFIdentifier UnclassifiedDocumentTypeValue { get; set; }
        [DataMember]
        [MFPropertyDef]
        [JsonConfEditor(Label = "Estado de procesamiento")]
        public MFIdentifier ProcessingStatusProperty { get; set; }
        [DataMember]
        [ValueSetter(
            Label = "Valor de estado de procesamiento",
            AllowedModes = new[] { TypedValueSettingMode.Dynamic, TypedValueSettingMode.Static, TypedValueSettingMode.SetToNULL },
            PropertyDefReferencePath = ".parent._children{.key == 'ProcessingStatusProperty' }")]
        public TypedValueSetter ProcessingStatusValue { get; set; }
        [MFWorkflow(Required = true, Validate = true, AllowEmpty = false)]
        [DataMember]
        [JsonConfEditor(Label = "Workflow")]
        public MFIdentifier Workflow { get; set; }
        [MFState(Required = true, Validate = true, AllowEmpty = false)]
        [DataMember]
        [JsonConfEditor(Label = "State", HelpText = "Estado \"No identificado\" en el workflow.")]
        public MFIdentifier State { get; set; }
    }
}
