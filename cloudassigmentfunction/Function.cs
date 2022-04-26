using CloudNative.CloudEvents;
using Google.Cloud.Functions.Framework;
using Google.Events.Protobuf.Cloud.PubSub.V1;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using RestSharp;
using Newtonsoft.Json;
using Google.Cloud.Storage.V1;
using System.Net.Http;
using System;
using Google.Cloud.Firestore;


namespace cloudassigmentfunction
{
        public class Function : ICloudEventFunction<MessagePublishedData>
    {
        private readonly ILogger _logger;

        public Function(ILogger<Function> logger) =>
            _logger = logger;

        public Task HandleAsync(CloudEvent cloudEvent, MessagePublishedData data, CancellationToken cancellationToken)
        {
            // string nameFromMessage = data.Message?.TextData;
            // string name = string.IsNullOrEmpty(nameFromMessage) ? "world" : nameFromMessage;
            // _logger.LogInformation("Hello {name}", name);

                _logger.LogInformation("Accessed the function");
                string nameFromMessage = data.Message?.TextData;

                dynamic myObj = JsonConvert.DeserializeObject(nameFromMessage);

                string fileBase64 = myObj.fileBase64.ToString();
                string fileNameFromPubSub = myObj.FileName.ToString();
                string fileId = myObj.Id.ToString();
                string fileOwnerEmail = myObj.FileOwnerEmail.ToString();

                _logger.LogInformation(fileBase64);

            	RestClient client = new RestClient ("https://getoutpdf.com/api/convert/document-to-pdf");
                RestRequest request = new RestRequest ();
                request.AddParameter ("api_key", "b6d63619f3c99231534ab07c27cd50c0aef35d55272407a2ed546c3cfbb08e4a");
                request.AddParameter("document",fileBase64);
                
                request.Method = Method.Post;
                var response = client.ExecuteAsync (request);

                response.Wait();

                 _logger.LogInformation(response.Result.Content);

                dynamic parsedReponse = JsonConvert.DeserializeObject(response.Result.Content);
                string  PdfBase64 = parsedReponse.pdf_base64.ToString();
                

                _logger.LogInformation("Base 64 for pdf: "+PdfBase64);
                //missing convert to pdf

                string bucketName = "cloudhomeassigmentbucket";
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileNameFromPubSub);
                string fileName = "Converted_"+fileNameWithoutExtension+".pdf";
                var storage = StorageClient.Create();

               
                 _logger.LogInformation("Converting pdf");
                byte[] pdfBytes = Convert.FromBase64String(PdfBase64);

                _logger.LogInformation("Connecting to file Bucket");

                FirestoreDb db = FirestoreDb.Create("cloudhomeassigment");
                DocumentReference docRef = db.Collection("users").Document(fileOwnerEmail).Collection("files").Document(fileId);

                 string convertedFileUrl = null;

                using(var ms = new MemoryStream(pdfBytes))
                {
                    
                    storage.UploadObject(bucketName, fileName, null,ms);
                    convertedFileUrl = $"https://storage.googleapis.com/{bucketName}/{fileName}";

                    _logger.LogInformation("Converted pdf uploaded");
                    
                }

                 docRef.UpdateAsync("ConvertedFileLink", convertedFileUrl).Wait();

                

            return Task.CompletedTask;
        }
    }
}
