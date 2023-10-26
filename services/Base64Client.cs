using Base64ApiClient.model;
using Base64ApiClient.util;
using MFiles.VaultApplications.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Base64ApiClient.services
{
    public class Base64Client
    {
        private ILogger Logger { get; } = LogManager.GetLogger(typeof(Base64Client));

        private Configuration configuration;

        private string SERVER;
        private string USERNAME;
        private string API_KEY;
        private Boolean USE_FLOW;
        private string FLOW_ID;

        private string RESOURCE_SCAN = "/api/scan";
        public Base64Client(Configuration configuration)
        {
            this.configuration = configuration;
            if (configuration.UseSandbbox)
            {
                Logger.Info("Ambiente Sanbox");
                this.SERVER = "https://base64.ai";
                this.USERNAME = this.configuration.Sandbox.Username;
                this.API_KEY = this.configuration.Sandbox.ApiKey;

                if(this.configuration.Sandbox.FlowID != null)
                {
                    this.FLOW_ID = this.configuration.Sandbox.FlowID;
                    this.USE_FLOW = true;
                }
                else
                {
                    this.USE_FLOW = false;
                }
            }
            else
            {
                this.Logger?.Info("Ambiente Productivo");
                this.SERVER = "https://base64.ai";
                this.USERNAME = this.configuration.Production.Username;
                this.API_KEY = this.configuration.Production.ApiKey;

                if (this.configuration.Production.FlowID != null)
                {
                    this.FLOW_ID = this.configuration.Production.FlowID;
                    this.USE_FLOW = true;
                }
                else
                {
                    this.USE_FLOW = false;
                }
            }
        }

        public async Task<DocumentToScanResponse> ScanDocument(DocumentToScanRequest document)
        {
            DocumentToScanResponse result = new DocumentToScanResponse();
            try
            {
                string apiUrl = this.SERVER + this.RESOURCE_SCAN;
                string apiKey = "ApiKey " + this.USERNAME + ":" + this.API_KEY;

                using (HttpClient httpClient = new HttpClient())
                {
                    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                    // Configurar la autenticación por API Key
                    httpClient.DefaultRequestHeaders.Add("Authorization", apiKey);

                    if(this.USE_FLOW)
                    {
                        httpClient.DefaultRequestHeaders.Add("base64ai-flow-id", this.FLOW_ID);
                    }

                    // Agregar headers personalizados (por ejemplo, un encabezado de contenido JSON)
                    httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                    
                    // Crear un objeto JSON a enviar en la solicitud (puedes personalizarlo según tus necesidades)
                    string jsonString = JsonConvert.SerializeObject(document);
                    var jsonContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

                    // Realizar una solicitud POST de ejemplo con datos JSON
                    HttpResponseMessage response = await httpClient.PostAsync(apiUrl, jsonContent);

                    if (response.IsSuccessStatusCode)
                    {
                        // Procesar la respuesta exitosa
                        string content = await response.Content.ReadAsStringAsync();
                        result.response = JsonUtils.DeserializeJsonArray(content);

                        this.Logger?.Info("[ RESPONSE ]: " + content);
                    }
                    else
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        this.Logger?.Debug("[ RESPONSE ]: " + content);
                        this.Logger?.Error("La solicitud marco error. Respuesta: " + content);
                    }
                }
            }
            catch(Exception e)
            {
                this.Logger?.Error(e.Message);
            }

            return result;
        }
    }
}
