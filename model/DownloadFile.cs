using MFiles.VaultApplications.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base64ApiClient.model
{
    public class DownloadFile
    {
        private ILogger Logger { get; } = LogManager.GetLogger(typeof(DownloadFile));
        public string DownloadFilePath { get; private set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public int FileID { get; set; }
        public string Base64Content { get; set; }
        public int Version { get; set; }
        private string FolderTemp;
        private string FileTemp;

        public DynamicObject Model { get; set; }

        public DownloadFile()
        {
            this.FolderTemp = Path.GetTempPath();
            Guid guid = Guid.NewGuid();
            this.FileTemp = Path.ChangeExtension(guid.ToString(), "pdf");
            this.DownloadFilePath = Path.Combine(FolderTemp, FileTemp);
        }

        public void calculateBase64()
        {
            string result = String.Empty;
            try
            {
                using (FileStream fileStream = File.OpenRead(this.DownloadFilePath))
                {
                    byte[] fileBytes = new byte[fileStream.Length];

                    // Leer el archivo en un arreglo de bytes
                    int bytesRead = fileStream.Read(fileBytes, 0, fileBytes.Length);

                    if (bytesRead > 0)
                    {
                        // Convertir los bytes en una cadena Base64
                        string base64String = Convert.ToBase64String(fileBytes, 0, bytesRead);

                        // Imprimir la cadena Base64
                        Logger.Debug("Representación Base64 del archivo:");
                        Logger.Debug(base64String);

                        Console.WriteLine("Representación Base64 del archivo:");
                        Console.WriteLine(base64String);
                        this.Base64Content = "data:application/pdf;base64," + base64String;
                    }
                    else
                    {
                        Logger.Warn("El archivo está vacío.");
                    }
                } // El bloque using cierra automáticamente el FileStream y libera recursos.
            }
            catch (FileNotFoundException e)
            {
                Logger.Error("El archivo no se encontró en la ubicación especificada. " + e.Message);
            }
            catch (IOException e)
            {
                Logger.Error("Ocurrió un error de entrada/salida: " + e.Message);
            }
            catch (Exception e)
            {
                Logger.Error("Ocurrió un error inesperado:  " + e.Message);
            }

        }
    }
}
