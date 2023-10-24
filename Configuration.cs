using Base64ApiClient.config;
using MFiles.VAF.Configuration;
using MFiles.VAF.Configuration.JsonAdaptor;
using MFiles.VAF.Extensions;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Base64ApiClient
{
    [DataContract]
    public class Configuration : MFiles.VAF.Extensions.Configuration.ConfigurationBase
    {
        [DataMember(Order = 1)]
        [JsonConfEditor(Label = "Usar ambiente sandbox")]
        public bool UseSandbbox { get; set; } = false;

        [DataMember(Order = 2)]
        [JsonConfEditor(
            Hidden = true,
            ShowWhen = ".parent._children{.key == 'UseSandbbox' && .value == true }", Label ="Ambiente Sandbox")]
        public ServiceEnvironment Sandbox { get; set; }

        [DataMember(Order = 3)]
        [JsonConfEditor(
            Hidden = true,
            ShowWhen = ".parent._children{.key == 'UseSandbbox' && .value == false }", Label = "Ambiente Productivo")]
        public ServiceEnvironment Production { get; set; }

        [DataMember(Order = 20)]
        [RecurringOperationConfiguration(VaultApplication.QueueId, VaultApplication.SearchDocumentsTaskType)]
        [JsonConfEditor(Label = "Intervalo de actualización(min)")]
        public TimeSpanEx Interval { get; set; } = new TimeSpanEx()
        {
            Interval = new TimeSpan(0, 1, 0),
            RunOnVaultStartup = false
        };

        [DataMember(Order = 4)]
        [JsonConfEditor(Label = "Condiciones de busqueda")]
        public SearchConditionsJA SearchConditionsDocuments;

        [DataMember(Order = 5)]
        [JsonConfEditor(Label = "Modelos")]
        public List<MappingModel> MappingModel { get; set; } = new List<MappingModel>();
    }

}
