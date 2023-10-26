using Base64ApiClient.config;
using Base64ApiClient.model;
using Base64ApiClient.services;
using MFiles.VAF.AppTasks;
using MFiles.VAF.Common;
using MFiles.VAF.Extensions.Configuration;
using MFiles.VaultApplications.Logging;
using MFilesAPI;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Base64ApiClient
{
    /// <summary>
    /// The entry point for this Vault Application Framework application.
    /// </summary>
    /// <remarks>Examples and further information available on the developer portal: http://developer.m-files.com/. </remarks>
    public class VaultApplication
        : MFiles.VAF.Extensions.ConfigurableVaultApplicationBase<Configuration>
    {
        private new ILogger Logger { get; set; }

        [TaskQueue]
        public const string QueueId = "Arkiva.VAF.Base64.AI.VaultApplication";
        public const string SearchDocumentsTaskType = "SearchDocuments";

        [TaskProcessor(QueueId, SearchDocumentsTaskType)]
        public void SearchDocuments(ITaskProcessingJob<TaskDirective> job)
        {
            job.Commit((transactionalVault) =>
            {

            });
            job.Update(percentComplete: 100, details: $"Completado");
            this.GetDocumentsToExtractData();
        }

        private void GetDocumentsToExtractData()
        {
            PropertyValue propertyValue = new PropertyValue();
            PropertyValues propertyValues = new PropertyValues();
            PropertyDef propertyDef;
            ObjectVersion objectVersion;

            List<ObjVerEx> documents = searchDocumentsToScan();
            
            foreach (ObjVerEx objVerEx in documents)
            {                
                ObjectVersionAndProperties versionAndProperties = this.PermanentVault.ObjectOperations.GetObjectVersionAndProperties(objVerEx.ObjVer);
                List<MFObjectFile> files = new List<MFObjectFile>();
                foreach (ObjectFile file in (IObjectFiles)versionAndProperties.VersionData.Files)
                    files.Add(new MFObjectFile(file));

                List<DownloadFile> downloadFiles = new List<DownloadFile>();
                foreach (MFObjectFile file in files)
                {
                    if (file.Extension.ToLower().Equals("pdf"))
                    {
                        DownloadFile document = new DownloadFile
                        {
                            Name = Regex.Replace(file.Title, "[^0-9A-Za-z_-]", ""),
                            Extension = file.Extension,
                            FileID = file.ID,
                            Version = file.Version
                        };
                        downloadFiles.Add(document);
                    }
                }

                foreach(DownloadFile document in downloadFiles)
                {
                    Logger.Info("ID documento: " + objVerEx.ID);
                    Logger.Info("Archivo: " + document.Name + "." + document.Extension);
                    Logger.Info("Archivo descargado: " + document.DownloadFilePath);
                    this.PermanentVault.ObjectFileOperations.DownloadFile(document.FileID, document.Version, document.DownloadFilePath);

                    document.calculateBase64();

                    DocumentToScanRequest documentToScan = new DocumentToScanRequest();
                    documentToScan.originalFileName = "(" + objVerEx.ID + "): " + document.Name + document.Extension;
                    documentToScan.document = document.Base64Content;

                    var client = new Base64Client(Configuration);
                    var tarea = client.ScanDocument(documentToScan);
                    DocumentToScanResponse response = tarea.Result;
                    document.Model = (DynamicObject)response.response[0];

                    var model = ((JToken)((DynamicObject)document.Model.GetProperty("model")).GetProperty("name")).Value<string>();
                    Logger.Info("Modelo: " + model);

                    MappingModel mapping = this.Configuration.MappingModel.Find((Predicate<MappingModel>)(mappingModel => mappingModel.ModelName == model));
                    propertyValues = new PropertyValues();
                    if (mapping != null)
                    {
                        Logger.Info("Modelo Indentificado: " + model);
                        foreach (MappingProperty property in mapping.Mappings)
                        {
                            var fieldValue = ((JToken)((DynamicObject)((DynamicObject)document.Model.GetProperty("fields")).GetProperty(property.FieldName)).GetProperty("value")).Value<string>();
                            Logger.Info(property.FieldName + ": " + fieldValue);

                            if(fieldValue != null)
                            {
                                propertyDef = this.PermanentVault.PropertyDefOperations.GetPropertyDef(property.PropertyName.ID);
                                propertyValue.PropertyDef = propertyDef.ID;
                                propertyValue.Value.SetValue(propertyDef.DataType, fieldValue);
                                propertyValues.Add(-1, propertyValue);
                            }
                        }

                        propertyDef = this.PermanentVault.PropertyDefOperations.GetPropertyDef(mapping.PropertyDocumentType.ID);
                        propertyValue.PropertyDef = propertyDef.ID;
                        propertyValue.Value.SetValue(propertyDef.DataType, mapping.DocumentTypeValue.ID);
                        propertyValues.Add(-1, propertyValue);

                        objectVersion = this.PermanentVault.ObjectOperations.CheckOut(objVerEx.ObjVer.ObjID);
                        this.PermanentVault.ObjectPropertyOperations.SetProperties(objectVersion.ObjVer, propertyValues);
                        this.PermanentVault.ObjectOperations.CheckIn(objectVersion.ObjVer);
                    }
                    else
                    {
                        Logger.Info("Modelo No configurado: " + model);
                        propertyDef = this.PermanentVault.PropertyDefOperations.GetPropertyDef(Configuration.UnclassifiedModel.UnclassifiedPropertyDocumentType.ID);
                        propertyValue.PropertyDef = propertyDef.ID;
                        propertyValue.Value.SetValue(propertyDef.DataType, Configuration.UnclassifiedModel.UnclassifiedDocumentTypeValue.ID);
                        propertyValues.Add(-1, propertyValue);

                        propertyDef = this.PermanentVault.PropertyDefOperations.GetBuiltInPropertyDef(MFBuiltInPropertyDef.MFBuiltInPropertyDefWorkflow);
                        propertyValue.PropertyDef = propertyDef.ID;
                        propertyValue.Value.SetValue(propertyDef.DataType, Configuration.UnclassifiedModel.Workflow.ID);
                        propertyValues.Add(-1, propertyValue);

                        propertyDef = this.PermanentVault.PropertyDefOperations.GetBuiltInPropertyDef(MFBuiltInPropertyDef.MFBuiltInPropertyDefState);
                        propertyValue.PropertyDef = propertyDef.ID;
                        propertyValue.Value.SetValue(propertyDef.DataType, Configuration.UnclassifiedModel.State.ID);
                        propertyValues.Add(-1, propertyValue);

                        objectVersion = this.PermanentVault.ObjectOperations.CheckOut(objVerEx.ObjVer.ObjID);
                        this.PermanentVault.ObjectPropertyOperations.SetProperties(objectVersion.ObjVer, propertyValues);
                        this.PermanentVault.ObjectOperations.CheckIn(objectVersion.ObjVer);
                    }
                }
            }
        }

        public VaultApplication()
        {
            // Populate the logger instance.
            //System.Diagnostics.Debugger.Launch();
            this.Logger = LogManager.GetLogger(this.GetType());
        }

        protected override void StartApplication()
        {
            // Initializing the log manager configures the vault-specific layout renderers (e.g. "${vault-guid}").
            // The optional configuration can be used to define where logs should be stored.
            // NOTE: it is best practice to load this from the configuration,
            // as shown on the technology-specific pages, e.g. https://developer.m-files.com/Frameworks/Logging/Vault-Application-Framework/
            LogManager.Initialize(this.PermanentVault, new NLogLoggingConfiguration());
        }

        private List<ObjVerEx> searchDocumentsToScan()
        {
            List<ObjVerEx> result = new List<ObjVerEx>();

            // Create our search builder.
            if (this.Configuration.SearchConditionsDocuments != null)
            {
                var builder = new MFSearchBuilder(this.PermanentVault, this.Configuration.SearchConditionsDocuments.ToApiObject(this.PermanentVault));
                result = builder.FindEx();
            }

            // Find items.
            return result;
        }

    }
}