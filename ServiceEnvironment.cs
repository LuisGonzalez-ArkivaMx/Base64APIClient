using MFiles.VAF.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Base64ApiClient
{
    [DataContract]
    [JsonConfEditor()]
    public class ServiceEnvironment
    {
        [DataMember(Order = 1)]
        [JsonConfEditor(Label = "Username")]
        public string Username { get; set; }

        [DataMember(Order = 2)]
        [JsonConfEditor(Label = "ApiKey")]
        [Security(ChangeBy = SecurityAttribute.UserLevel.VaultAdmin)]
        public string ApiKey { get; set; }

        [DataMember(Order = 3)]
        [JsonConfEditor(Label = "Flow ID")]
        [Security(ChangeBy = SecurityAttribute.UserLevel.VaultAdmin)]
        public string FlowID { get; set; }
    }
}
