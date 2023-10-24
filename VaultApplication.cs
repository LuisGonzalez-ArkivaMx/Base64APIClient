using Base64ApiClient.model;
using Base64ApiClient.services;
using MFiles.VAF.AppTasks;
using MFiles.VAF.Common;
using MFiles.VAF.Extensions.Configuration;
using MFiles.VaultApplications.Logging;
using MFilesAPI;
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
        private ILogger Logger { get; } = LogManager.GetLogger(typeof(VaultApplication));

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
                    this.PermanentVault.ObjectFileOperations.DownloadFile(document.FileID, document.Version, document.DownloadFilePath);
                    document.calculateBase64();

                    DocumentToScanRequest documentToScan = new DocumentToScanRequest();
                    documentToScan.originalFileName = document.Name + document.Extension;
                    documentToScan.document = document.Base64Content;

                    var client = new Base64Client(Configuration);
                    var tarea = client.ScanDocument(documentToScan);
                    DocumentToScanResponse response = tarea.Result;
                    document.Model = (DynamicObject)response.response[0];
                    Logger.Info("Model: " + ((DynamicObject)document.Model.GetProperty("model")).GetProperty("name"));
                    if(((DynamicObject)document.Model.GetProperty("model")).GetProperty("name").Equals("CURP"))
                    {
                        Logger.Info("CURP: " + ((DynamicObject)((DynamicObject)document.Model.GetProperty("fields")).GetProperty("clave")).GetProperty("value"));
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